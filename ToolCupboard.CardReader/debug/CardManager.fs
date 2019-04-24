namespace ToolCupboard.CardReader.Debug

open ToolCupboard.CardReader

type CardManager() =
    let cardInserted = new Event<_>()
    let cardRemoved = new Event<_>()

    member val private Context = null with get,set

    member val private Monitors = [] with get,set

    interface ICardManager with
        [<CLIEvent>]
        member this.CardInserted = cardInserted.Publish

        [<CLIEvent>]
        member this.CardRemoved = cardRemoved.Publish

        member this.Initialize() = ()

    member this.TriggerCardInserted(cardid) =
        cardInserted.Trigger(this :> obj, CardEventArgs(CardId = cardid))

    member this.TriggerCardRemoved(cardid) =
        cardRemoved.Trigger(this :> obj, CardEventArgs(CardId = cardid))
