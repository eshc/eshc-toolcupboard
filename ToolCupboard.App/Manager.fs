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
        async {
            do! Async.SwitchToContext(syncContext)

            let popup = wnd.PopupMessageBox("Detected Card...", false)

            let! log = AccessLog.LogAsync None args.CardId |> Async.StartChild
            // look up if it was a tool or a user
            let! user = Users.LookupUserAsync None args.CardId |> Async.StartChild
            let! tool = Tools.LookupToolAsync None args.CardId |> Async.StartChild

            let page = wnd.PageControl.Content :?> ICardHandler

            let! user = user
            match user with 
            | None -> 
                let! tool = tool
                match tool with
                | None -> 
                    popup.SetText "Unknown Card"
                    popup.StartFade()
                | Some tool -> popup.SetText "Tool"
            | Some user -> 
                page.HandleUser user popup

            do! log
        } |> fun v -> Async.StartImmediate(v)

    member this.Initialize() =
        this.CardManager.Initialize()

        this.CardManager.CardInserted.Add(this.CardInserted)
