module ToolCupboard.Database.Tools

open ToolCupboard.Database.Users
open ToolCupboard.Database.Provider

type Tool = Db.dataContext.``public.toolsEntity``
type ToolCheckout = Db.dataContext.``public.tool_checkoutEntity``
type BorrowOrReturnResult = Borrowed | Returned 

val RegisterToolAsync : ctxt:Db.dataContext option -> cardId:string -> Async<Tool>
val LookupToolAsync : ctxt:Db.dataContext option -> cardId:string -> Async<Tool option>
val BorrowOrReturnToolAsync : ctxt:Db.dataContext option -> tool:Tool -> user:User -> toolCardId:string -> userCardId:string -> Async<BorrowOrReturnResult>