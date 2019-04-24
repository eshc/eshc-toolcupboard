namespace ToolCupboard.CardReader

type CardEventArgs() =
    member val CardId = "" with get, set
    member val Handled = false with get, set
