module ToolCupboard.Database.Tools

open ToolCupboard.Database.Users
open ToolCupboard.Database.Provider

type Tool = Db.dataContext.``public.toolsEntity``
type ToolCheckout = Db.dataContext.``public.tool_checkoutEntity``
type CheckInOutResult = CheckedIn | CheckedOut

val LookupToolAsync : ctxt:Db.dataContext option -> cardId:string -> Async<Tool option>
val CheckInOrOutToolAsync : ctxt:Db.dataContext option -> tool:Tool -> user:User -> userCardId:string -> toolCardId:string -> Async<CheckInOutResult>