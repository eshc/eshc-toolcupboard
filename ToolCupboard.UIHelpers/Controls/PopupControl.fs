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

    static member val AutoFadeProperty = AvaloniaProperty.Register<PopupControl, bool>("AutoFade")

    member this.AutoFade
        with get () = this.GetValue(PopupControl.AutoFadeProperty)
        and set v = this.SetValue(PopupControl.AutoFadeProperty, v)

    member this.FadeAsync() =
        async {
            do! Async.Sleep(this.Delay)
            let panel = this.Ancestor<Panel>()
            Option.ofObj panel |> Option.iter (fun panel -> 
                panel.Children.Remove(this) |> ignore
            )
        }

    member this.StartFade() =
        this.FadeAsync() |> Async.StartImmediate

    member this.OnInitialized(sender, args) =
        if this.AutoFade then
            this.StartFade()

    override this.OnAttachedToVisualTree(b) =
        base.OnAttachedToVisualTree(b)

    member this.SetText(text) =
        match this.Content with
        | :? TextBlock as tb -> tb.Text <- text
        | _ -> 
            let content = TextBlock(Classes=Classes.Parse("h2"), Text="text")
            this.Content <- content



