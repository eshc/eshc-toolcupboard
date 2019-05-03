namespace ToolCubpoard.Website.Services

open ToolCupboard.LDAP

type LdapService() =
    let par = LdapParameters()
    let ldap = new LdapHelper(par)

    member this.Authenticate user pass = 
        ldap.Authenticate user pass

    interface ILdapService with
        member this.Authenticate user pass = this.Authenticate user pass