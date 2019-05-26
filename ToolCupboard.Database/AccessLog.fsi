module ToolCupboard.Database.AccessLog

open System
open ToolCupboard.Database.Provider

val LogAsync : ctxt:Db.dataContext option -> cardId:string -> Async<unit>

type UnknownCard = {
    CardId : string;
    LastDate : DateTime;
}

val LastUnknownCards : ctxt:Db.dataContext option -> c:int ->  Async<seq<UnknownCard>>