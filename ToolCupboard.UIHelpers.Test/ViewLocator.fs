namespace ToolCupboard.UIHelpers.Test 

open System
open Avalonia.Controls
open Avalonia.Controls.Templates
open ToolCupboard.UIHelpers.Test.ViewModels

type ViewLocator() =
    interface IDataTemplate with
        member val SupportsRecycling = false

        member this.Build(data) =
            let name = data.GetType().FullName.Replace("ViewModel", "View")
            let typ = Type.GetType(name)

            if isNull typ
            then (TextBlock(Text = "Not Found:" + name) :> IControl)
            else (Activator.CreateInstance(typ) :?> IControl)

        member this.Match(data:obj) =
            data :? ViewModelBase

