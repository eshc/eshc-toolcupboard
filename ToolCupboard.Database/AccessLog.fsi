module ToolCupboard.Database.AccessLog

open ToolCupboard.Database.Provider

val LogAsync : ctxt:Db.dataContext option -> cardId:string -> Async<unit>