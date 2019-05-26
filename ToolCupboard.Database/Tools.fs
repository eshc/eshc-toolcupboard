module ToolCupboard.Database.Tools

open System
open FSharp.Data.Sql
open ToolCupboard.Database.Users
open ToolCupboard.Database.Provider

type BorrowOrReturnResult = Borrowed | Returned 

let RegisterToolAsync ctxt cardId = 
    let ctxt = Option.defaultWith (Db.GetDataContext) ctxt
    let tool = ctxt.Public.Tools.Create("Unknown Tool", "Unknown Location", "Unknown Tool")
    async {
        do! ctxt.SubmitUpdatesAsync()
        tool.Name <- sprintf "Unknown Tool %d" tool.ToolId
        let card = ctxt.Public.ToolCards.Create(DateTime.Now, "Unknown Card", tool.ToolId)
        card.CardId <- cardId
        do! ctxt.SubmitUpdatesAsync()
        return tool
    }

let LookupToolAsync ctxt cardId =
    let ctxt = Option.defaultWith (Db.GetDataContext) ctxt
    query {
        for t in ctxt.Public.ToolCards do
        where (t.CardId = cardId)
        for tool in t.``public.tools by tool_id`` do
        select tool
    } |> Seq.tryHeadAsync

let BorrowOrReturnToolAsync ctxt (tool:Tool) (user:User) toolCardId userCardId =
    let ctxt = Option.defaultWith (Db.GetDataContext) ctxt
    async {
        let! old = 
            query {
                for t in ctxt.Public.ToolCheckout do
                where (t.ToolId = tool.ToolId)
                select t
            } |> Seq.tryHeadAsync
        let v = 
            match old with
            | None -> 
                let tc = ctxt.Public.ToolCheckout.Create(DateTime.Now, user.UserId)
                tc.ToolId <- tool.ToolId
                ctxt.Public.ToolLog.Create(true, DateTime.Now, toolCardId, tool.ToolId, userCardId, user.UserId) |> ignore
                Borrowed 
            | Some old ->
                old.Delete()
                ctxt.Public.ToolLog.Create(false, DateTime.Now, toolCardId, tool.ToolId, userCardId, user.UserId) |> ignore
                Returned
        do! ctxt.SubmitUpdatesAsync()
        return v
    }

let GetToolAsync ctxt id =
    async {
        let ctxt = Option.defaultWith (Db.GetDataContext) ctxt
        let q = 
            query {
                for tool in ctxt.Public.Tools do
                where (tool.ToolId = id)
                select tool
                exactlyOneOrDefault
            } |> Option.ofObj
        return q
    }
