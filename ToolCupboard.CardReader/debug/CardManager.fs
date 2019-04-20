namespace ToolCupboard.CardReader.Debug

open ToolCupboard.CardReader

type CardManager() =
    member val private Context = null with get,set

    member val private Monitors = [] with get,set

    interface ICardManager with
        member this.Initialize() = ()
        member val CardInserted = new Event<_>()
        member val CardRemoved = new Event<_>()

    member this.TriggerCardInserted(cardid) =
        (this :> ICardManager).CardInserted.Trigger(this :> obj, CardEventArgs(CardId = cardid))

    member this.TriggerCardRemoved(cardid) =
        (this :> ICardManager).CardRemoved.Trigger(this :> obj, CardEventArgs(CardId = cardid))