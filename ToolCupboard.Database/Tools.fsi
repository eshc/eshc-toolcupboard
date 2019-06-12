module ToolCupboard.Database.Tools

open ToolCupboard.Database.Users
open ToolCupboard.Database.Provider

type BorrowOrReturnResult = Borrowed | Returned 

val RegisterToolAsync : ctxt:Context -> cardId:string -> Async<Tool>
val LookupToolAsync : ctxt:Context -> cardId:string -> Async<Tool option>
val BorrowOrReturnToolAsync : ctxt:Context -> tool:Tool -> user:User -> toolCardId:string -> userCardId:string -> Async<BorrowOrReturnResult>
val GetToolAsync : ctxt:Context -> id:int64 -> Async<Tool option>
val DeleteToolCardAsync : ctxt:Context -> id:string -> Async<ToolCard option>