module ToolCupboard.Database.Users

open ToolCupboard.Database.Provider

type User = Db.dataContext.``public.usersEntity``
type Tool = Db.dataContext.``public.toolsEntity``

val LookupUserAsync : ctxt:Db.dataContext option -> cardId:string -> Async<User option>
val LookupUserToolsAsync : ctxt:Db.dataContext option -> user:User -> Async<seq<Tool>>