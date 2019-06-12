namespace ToolCupboard.App.ViewModels

open ToolCupboard.Database.Provider
open ToolCupboard.UIHelpers.ViewModels

type ToolViewModel(tool : Tool) =
    inherit ViewModelBase()

    member val Tool = tool

    member this.Name with get () = this.Tool.Name