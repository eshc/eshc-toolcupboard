module ToolCupboard.UIHelpers.Test.Program

open System
open Avalonia
open Avalonia.Logging.Serilog
open ToolCupboard.UIHelpers.Test.ViewModels
open ToolCupboard.UIHelpers.Test.Views

[<EntryPoint>]
let main argv =
    let dcp () = MainWindowViewModel() :> Object
    ToolCupboard.UIHelpers.Library.load ()
    AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .LogToDebug()
        .Start<MainWindow>(dataContextProvider = Func<Object>(dcp))
    0
