namespace ToolCupboard.CardReader

open ToolCupboard.CardReader

type ICardManager =
    abstract member Initialize: unit -> unit

    abstract member CardInserted: Event<obj * CardEventArgs>
    abstract member CardRemoved: Event<obj * CardEventArgs>