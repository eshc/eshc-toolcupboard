namespace ToolCupboard.CardReader

open ToolCupboard.CardReader

type ICardManager =
    abstract member Initialize: unit -> unit

    [<CLIEvent>]
    abstract member CardInserted: IEvent<obj * CardEventArgs>

    [<CLIEvent>]
    abstract member CardRemoved: IEvent<obj * CardEventArgs>