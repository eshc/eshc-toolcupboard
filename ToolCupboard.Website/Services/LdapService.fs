namespace ToolCubpoard.Website.Services

open ToolCupboard.LDAP
open Microsoft.Extensions.Configuration

type LdapService(config : IConfiguration) =
    let par = 
        LdapParameters(
            Host = config.["LDAP:Host"], 
            Port = int config.["LDAP:Port"],
            RootDN = config.["LDAP:RootDN"],
            ManagerCN = config.["LDAP:Bind:User"],
            ManagerPW = config.["LDAP:Bind:Pass"],
            NameField = config.["LDAP:Properties:Name"],
            MemberOfField = config.["LDAP:Properties:MemberOf"],
            EmailField = config.["LDAP:Properties:Email"],
            UsernameField = config.["LDAP:Properties:Username"],
            MembersOU = config.["LDAP:MembersOU"]
            )

    let ldap = new LdapHelper(par) 

    member this.Authenticate user pass = 
        ldap.Authenticate user pass

    interface ILdapService with
        member this.Authenticate user pass = this.Authenticate user pass