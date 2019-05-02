open Avalonia
open Avalonia.Controls
open Avalonia.Logging.Serilog
open ToolCupboard.App
open ToolCupboard.App.Views
open ToolCupboard.App.Pages
open ToolCupboard.UIHelpers.Views

let appmain debug (app : Application) (argv : string []) : unit =
    let wnd = PageWindow(Width=480.0,Height=800.0)
    wnd.Navigate(Pages.LockedPage())
    let mgr = Manager(wnd, debug)
    mgr.Initialize()
    if debug then
        let dbgWnd = DebugCardWindow(mgr)
        dbgWnd.Show()
    app.Run(wnd)

[<EntryPoint>]
let main argv =
    let _ = ToolCupboard.UIHelpers.Controls.PopupControl.DelayProperty
    let debug = false
    let app = App()
    let appmain = appmain debug |> fun v -> AppBuilderBase<AppBuilder>.AppMainDelegate(v)
    AppBuilder.Configure(app)
        .UsePlatformDetect()
        .LogToDebug()
        .Start(appmain, argv)
    0
