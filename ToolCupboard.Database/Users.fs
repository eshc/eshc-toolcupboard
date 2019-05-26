module ToolCupboard.Database.Users

open System
open FSharp.Data.Sql
open ToolCupboard.Database.Provider

let LookupUserAsync ctxt cardId =
    let ctxt = Option.defaultWith (Db.GetDataContext) ctxt
    query {
        for t in ctxt.Public.UserCards do
        where (t.CardId = cardId)
        for user in t.``public.users by user_id`` do
        select user
    } |> Seq.tryHeadAsync

let LookupUserToolsAsync ctxt (user:User) =
    let ctxt = Option.defaultWith (Db.GetDataContext) ctxt
    async {
        let! tools = 
            query {
                for t in ctxt.Public.ToolCheckout do
                where (t.UserId = user.UserId)
                for tool in t.``public.tools by tool_id`` do
                select tool
            } |> Seq.executeQueryAsync
        return tools
    }

let LogUserAsync ctxt (user:User) cardId = 
    let ctxt = Option.defaultWith (Db.GetDataContext) ctxt
    ctxt.Public.UserLog.Create(cardId, DateTime.Now, user.UserId) |> ignore
    ctxt.SubmitUpdatesAsync()

let GetUserAsync ctxt id =
    async {
        let ctxt = Option.defaultWith (Db.GetDataContext) ctxt
        let user = 
            query {
                for user in ctxt.Public.Users do
                where (user.UserId = id)
                select user
                exactlyOneOrDefault
            } |> Option.ofObj
        return user
    }

let DeleteUserCardAsync ctxt id =
    async {
        let ctxt = Option.defaultWith (Db.GetDataContext) ctxt
        let card = 
            query {
                for card in ctxt.Public.UserCards do
                where (card.CardId = id)
                select card
                exactlyOneOrDefault
            } |> Option.ofObj
        card |> Option.iter (fun c -> c.Delete ())
        do! ctxt.SubmitUpdatesAsync()
        return card
    }
