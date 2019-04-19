namespace ToolCupboard.UIHelpers.Controls

open Avalonia.Controls

type ToolWindow() =
    inherit Window()

    member this.Overlay with get () = this.FindControl<Panel>("overlay")

    member this.PopupMessageBox(text) = 
        let overlay = this.Overlay
        let content = TextBlock(Classes=Classes.Parse("h2"), Text="text")
        overlay.Children.Add(new PopupControl(Content = content))
        ()