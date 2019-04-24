namespace ToolCupboard.UIHelpers.Controls

open Avalonia.Controls
open Avalonia.Markup.Xaml
open ToolCupboard.UIHelpers.Controls
open ToolCupboard.UIHelpers.VisualTreeExtensions

type PageControl() =
    inherit ContentControl()

    member this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)

    member val PageNavigation = PageNavigation()

    member this.Navigate(page) =
        this.PageNavigation.Navigate(this, page)
        this.InvalidateVisual()

    member this.GoBack() =
        this.PageNavigation.GoBack(this)

    member this.CanGoBack with get () = this.PageNavigation.CanGoBack

    member this.Overlay
        with get () =
            this.BreadthFirstFindByName<StackPanel>("overlay")
            |> Option.toObj

    member this.PopupMessageBox(text, ?autoFade) =
        let overlay = this.Overlay
        let content = TextBlock(Classes=Classes.Parse("h2"), Text="text")
        let popup = PopupControl(Content = content)
        popup.AutoFade <- Option.defaultValue true autoFade
        overlay.Children.Add(popup)
        popup
