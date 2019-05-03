module ToolCupboard.Database.AccessLog

open System
open System.Linq
open FSharp.Data.Sql
open ToolCupboard.Database.Provider

type AccessLog = Db.dataContext.``public.access_logEntity``

type UnknownCard = {
    CardId : string;
    LastDate : DateTime;
}

let LogAsync ctxt cardId =
    let ctxt = Option.defaultWith Db.GetDataContext ctxt
    ctxt.Public.AccessLog.Create(cardId, DateTime.Now) |> ignore
    ctxt.SubmitUpdatesAsync()
  
let LastUnknownCards ctxt (c:int) = 
    let ctxt = Option.defaultWith Db.GetDataContext ctxt
    async {
        let qusercards = query {
            for card in ctxt.Public.UserCards do
            select card.CardId
            distinct
        }
        let qtoolcards = query {
            for card in ctxt.Public.UserCards do
            select card.CardId
            distinct
        }
        let! tools = 
            query {
                for access in ctxt.Public.AccessLog do
                groupBy access.CardId into g
                (* where (
                    not (qusercards.Contains(g.Key)) 
                    && not (qtoolcards.Contains(g.Key))) *)
                select { CardId = g.Key; LastDate = g.Max(fun v -> v.Date)}
                take c
            } |> Seq.executeQueryAsync
        return tools
    }