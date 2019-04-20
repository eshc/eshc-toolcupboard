// Learn more about F# at http://fsharp.org

open Avalonia
open Avalonia.Logging.Serilog
open System
open ToolCupboard.App

[<EntryPoint>]
let main argv =
    AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .LogToDebug()
        .Start<MainWindow>()
    0 // return an integer exit code
