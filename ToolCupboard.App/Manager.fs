namespace ToolCupboard.App

open Avalonia.Threading
open ToolCupboard.CardReader
open ToolCupboard.Database
open ToolCupboard.UIHelpers.Views
open Avalonia.Threading


type Manager(wnd: PageWindow, debug) =
    let syncContext = AvaloniaSynchronizationContext.Current

    let cardManager =
        if debug
        then Debug.CardManager() :> ICardManager
        else PCSC.CardManager() :> ICardManager

    member val CardManager = cardManager with get

    member this.CardInserted(obj, args : CardEventArgs) = 
        let cardId = args.CardId
        async {
            do! Async.SwitchToContext(syncContext)

            let popup = wnd.PopupMessageBox("Detected Card...", autoFade=false)

            let! log = AccessLog.LogAsync None cardId |> Async.StartChild
            // look up if it was a tool or a user
            let! user = Users.LookupUserAsync None cardId |> Async.StartChild
            let! tool = Tools.LookupToolAsync None cardId |> Async.StartChild

            let page = wnd.PageControl.Content :?> ICardHandler

            let! user = user
            match user with 
            | None -> 
                let! tool = tool
                match tool with
                | None -> 
                    do! page.HandleUnknown cardId popup
                | Some tool ->
                    do! page.HandleTool tool cardId popup
            | Some user -> 
                let! log = Users.LogUserAsync None user cardId |> Async.StartChild
                do! page.HandleUser user cardId popup
                do! log

            do! log
        } |> fun v -> Async.StartImmediate(v)

    member this.Initialize() =
        this.CardManager.Initialize()

        this.CardManager.CardInserted.Add(this.CardInserted)
