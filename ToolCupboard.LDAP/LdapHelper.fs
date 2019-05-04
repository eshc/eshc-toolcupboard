namespace ToolCupboard.LDAP

open ToolCupboard.Common
open ToolCupboard.Common.CompExpr
open System.Text.RegularExpressions
open Novell.Directory.Ldap
open System
open Novell.Directory.Ldap.Rfc2251
open System.Text
open Novell.Directory.Ldap
open Novell.Directory.Ldap

type LdapUserInfo = {
    dn : string;
    displayName : string;
    email : string;
    username : string;
    groups : seq<string>;
}

type LdapParameters() =
    member val Host = "localhost" with get, set
    member val Port = 389 with get, set
    member val RootDN = "dc=directory,dc=eshc,dc=coop" with get, set
    member val ManagerCN = "cn=Manager" with get, set
    member val ManagerPW = "" with get, set

    member val NameField = "displayName" with get, set
    member val MemberOfField = "memberOf" with get, set
    member val EmailField = "mail" with get, set
    member val UsernameField = "uid" with get, set

    member val MembersOU = "ou=Members" with get, set

    member this.DN subdn = sprintf "%s,%s" subdn this.RootDN

    member this.ManagerDN with get () = this.DN this.ManagerCN

    member this.TrimGroupName (dn:string) =
        seq {
            for gr in dn.Split [| ',' |] do
                let spl = gr.Split [|'='|]
                yield if spl.Length > 1 then spl.[1] else spl.[0]
        } |> Seq.head

    member this.GroupNameToDN groupName =
        sprintf "cn=%s,ou=Groups,%s" groupName this.RootDN

type LdapError = ConnectError | BindError | InputError | NoSingleEntry | IncorrectManagerPassword | IncorrectPassword

type LdapFilter() =
    let filter = RfcFilter()

    member this.Encode(s : string) = Encoding.UTF8.GetBytes(s)

    member this.Filter(ty, f) =
        filter.StartNestedFilter(ty)
        f this |> ignore
        filter.EndNestedFilter(ty)
        this

    member this.And(f) = 
        this.Filter(RfcFilter.And, f)

    member this.Or(f) = 
        this.Filter(RfcFilter.Or, f)

    member this.Equals(key, value) = 
        filter.AddAttributeValueAssertion(RfcFilter.EqualityMatch, key, this.Encode value)
        this

    member this.Get with get () = filter

type LdapHelper(par : LdapParameters) =
    let connection = new LdapConnection()

    
    interface IDisposable with 
        member this.Dispose() = 
            connection.Dispose()

    static member ldapFieldRegex = Regex(@"[a-zA-Z0-9@_-]*")

    member this.ValidateLdapField value = 
        if LdapHelper.ldapFieldRegex.IsMatch(value)
        then Result.Ok value
        else Result.Error LdapError.InputError

    member this.Connect(connection : LdapConnection) =
        execresult {
            connection.Connect(par.Host, par.Port)
            do! Result.ofBool connection.Connected LdapError.ConnectError
        }

    member this.Bind(connection : LdapConnection, dn, pw) =
        execresult { 
            do!
                try connection.Bind(dn, pw) |> Result.result
                with | :? LdapException -> Result.error LdapError.IncorrectPassword
            do! Result.ofBool connection.Bound LdapError.BindError
        }

    member this.Connection 
        with get () = 
            execresult {
                if not connection.Connected then 
                    do! this.Connect(connection)
                    do! this.Bind(connection, par.ManagerDN, par.ManagerPW) 
                        |> Result.mapError (function IncorrectPassword -> IncorrectManagerPassword | v -> v)
                return connection
            }

    member this.Search(baseDN, filter : LdapFilter, attrs, ?scope, ?namesOnly) =
        let scope = defaultArg scope LdapConnection.ScopeOne
        let namesOnly = defaultArg namesOnly false
        let attrs = Array.ofSeq attrs
        let filter = filter.Get
        LdapSearchRequest(baseDN, scope, filter, attrs, LdapSearchConstraints.DerefAlways, 512, 1, namesOnly, null)

    member this.ResponseResults(res : LdapMessageQueue) =
        seq {
            let tryagain = ref true 
            while !tryagain do 
                let r = res.GetResponse()
                if isNull r then tryagain := false
                elif r :? LdapSearchResult then yield (r :?> LdapSearchResult).Entry
        }

    member this.GetUserInfo username = 
        execresult {
            let! connection = this.Connection            
            let! user = this.ValidateLdapField username

            let filter = LdapFilter().And(fun f -> 
                f.Equals("objectClass", "inetOrgPerson").Or(fun f ->
                    f.Equals(par.EmailField, username)
                        .Equals(par.UsernameField, username)))
            let req = this.Search(par.DN par.MembersOU, filter, [par.NameField; par.UsernameField; par.EmailField; par.MemberOfField])
            let res = connection.SendRequest(req, null)
            let entries = this.ResponseResults(res) |> List.ofSeq
            let! user = Result.tryErr (fun () -> List.exactlyOne entries) LdapError.NoSingleEntry
            let dn = user.Dn

            let groups = 
                user.GetAttribute(par.MemberOfField).StringValueArray
                |> Array.map par.TrimGroupName
            let name = user.GetAttribute(par.NameField).StringValue |> Option.ofObj |> Option.defaultValue dn
            let email = user.GetAttribute(par.EmailField).StringValue
            let username = user.GetAttribute(par.UsernameField).StringValue

            return { dn = dn; displayName = name; username = username; email = email; groups = groups } 
        }

    member this.Authenticate username password =
        execresult {
            let! user = this.GetUserInfo username

            use conn = new LdapConnection(SecureSocketLayer = false)
            do! this.Connect(conn)
            do! this.Bind(conn, user.dn, password)

            return user
        }

