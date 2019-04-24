namespace ToolCupboard.UIHelpers.Test.ViewModels

open System
open System.Collections.Generic
open System.Text
open ToolCupboard.UIHelpers.ViewModels

type MainWindowViewModel() =
    inherit ViewModelBase()

    member val Greeting = "Hello World!" with get,set
