namespace ToolCupboard.UIHelpers.Controls

open System
open System.Threading
open Avalonia
open Avalonia.Controls
open Avalonia.Markup.Xaml
open System.Threading.Tasks
open ToolCupboard.UIHelpers.VisualTreeExtensions

type PopupControl() as this =
    inherit ContentControl()

    do this.Initialized.AddHandler(fun o v -> this.OnInitialized(o,v))

    static member val DelayProperty = AvaloniaProperty.Register<PopupControl, int>("Delay")

    member this.Delay
        with get () = this.GetValue(PopupControl.DelayProperty)
        and set v = this.SetValue(PopupControl.DelayProperty, v)

    member this.FadeAsync() =
        async {
            do! Async.Sleep(this.Delay)
            let panel = this.Ancestor<Panel>()
            panel.Children.Remove(this) |> ignore
        }

    member this.OnInitialized(sender, args) =
        this.FadeAsync() |> Async.StartImmediate

    override this.OnAttachedToVisualTree(b) =
        base.OnAttachedToVisualTree(b)


