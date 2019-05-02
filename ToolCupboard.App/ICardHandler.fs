namespace ToolCupboard.App

open ToolCupboard.Database.Users
open ToolCupboard.UIHelpers.Controls

type ICardHandler =
    abstract member HandleUser : user:User -> cardId:string -> popup:PopupControl -> Async<unit>
    abstract member HandleTool : tool:Tool -> cardId:string -> popup:PopupControl -> Async<unit>
    abstract member HandleUnknown : cardId:string -> popup:PopupControl -> Async<unit>