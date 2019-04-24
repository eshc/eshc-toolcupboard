namespace ToolCupboard.App

open ToolCupboard.Database.Users
open ToolCupboard.UIHelpers.Controls

type ICardHandler =
    abstract member HandleUser : user:User -> popup:PopupControl -> unit
    abstract member HandleTool : tool:Tool -> popup:PopupControl -> unit
    abstract member HandleUnknown : cardId:string -> popup:PopupControl -> unit