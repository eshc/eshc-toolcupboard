namespace ToolCubpoard.Website.Services

open ToolCupboard.LDAP

type ILdapService =
    abstract member Authenticate : string -> string -> Result<LdapUserInfo, LdapError>
    abstract member GetUserInfo : user:string -> Result<LdapUserInfo, LdapError>