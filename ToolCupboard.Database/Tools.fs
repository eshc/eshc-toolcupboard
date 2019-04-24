module ToolCupboard.Database.Tools

open System
open FSharp.Data.Sql
open ToolCupboard.Database.Users
open ToolCupboard.Database.Provider

type Tool = Db.dataContext.``public.toolsEntity``
type ToolCheckout = Db.dataContext.``public.tool_checkoutEntity``
type CheckInOutResult = CheckedIn | CheckedOut

let LookupToolAsync ctxt cardId =
    let ctxt = Option.defaultWith (Db.GetDataContext) ctxt
    query {
        for t in ctxt.Public.ToolCards do
        where (t.CardId = cardId)
        for tool in t.``public.tools by tool_id`` do
        select tool
    } |> Seq.tryHeadAsync

let CheckInOrOutToolAsync ctxt (tool:Tool) (user:User) userCardId toolCardId =
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
                CheckedIn 
            | Some old ->
                old.Delete()
                ctxt.Public.ToolLog.Create(false, DateTime.Now, toolCardId, tool.ToolId, userCardId, user.UserId) |> ignore
                CheckedOut
        do! ctxt.SubmitUpdatesAsync()
        return v
    }