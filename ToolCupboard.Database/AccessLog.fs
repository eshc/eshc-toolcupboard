module ToolCupboard.Database.AccessLog

open System
open ToolCupboard.Database.Provider

let LogAsync ctxt cardId =
    let ctxt = Option.defaultWith Db.GetDataContext ctxt
    ctxt.Public.AccessLog.Create(cardId, DateTime.Now) |> ignore
    ctxt.SubmitUpdatesAsync()
  