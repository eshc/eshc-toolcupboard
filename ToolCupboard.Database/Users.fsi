module ToolCupboard.Database.Users

open ToolCupboard.Database.Provider

val LogUserAsync : ctxt:Db.dataContext option -> user:User -> cardId:string -> Async<unit>
val LookupUserAsync : ctxt:Db.dataContext option -> cardId:string -> Async<User option>
val LookupUserToolsAsync : ctxt:Db.dataContext option -> user:User -> Async<seq<Tool>>
val GetUserAsync : ctxt:Context -> id:int64 -> Async<User option>
val DeleteUserCardAsync : ctxt:Context -> id:string -> Async<UserCard Option>