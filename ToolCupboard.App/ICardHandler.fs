namespace ToolCupboard.App

open ToolCupboard.Database.Provider
open ToolCupboard.Gpio
open ToolCupboard.UIHelpers.Controls

type ICardHandler =
    abstract member HandleUser : mgr:GpioManager -> user:User -> cardId:string -> popup:PopupControl -> Async<unit>
    abstract member HandleTool : mgr:GpioManager -> tool:Tool -> cardId:string -> popup:PopupControl -> Async<unit>
    abstract member HandleUnknown : mgr:GpioManager -> cardId:string -> popup:PopupControl -> Async<unit>
    abstract member HandleDoorClosed : mgr:GpioManager -> Async<unit>
    abstract member HandleDoorOpened : mgr:GpioManager -> Async<unit>