namespace ToolCupboard.UIHelpers.Views

open Avalonia
open Avalonia.Controls
open Avalonia.Markup.Xaml
open ToolCupboard.UIHelpers.Controls

type PageWindow() =
    inherit Window()

    member this.InitializeComponent() =
        ()

    member val PageNavigation = PageNavigation()

    member this.Navigate(page) =
        this.PageNavigation.Navigate(this, page)

    member this.GoBack(page) =
        this.PageNavigation.GoBack(this)

    member this.CanGoBack with get () = this.PageNavigation.CanGoBack

    member this.Overlay with get () = this.FindControl<Panel>("overlay")

    member this.PopupMessageBox(text) =
        let overlay = this.Overlay
        let content = TextBlock(Classes=Classes.Parse("h2"), Text="text")
        overlay.Children.Add(new PopupControl(Content = content))
        ()
