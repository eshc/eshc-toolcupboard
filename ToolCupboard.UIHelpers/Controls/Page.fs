namespace ToolCupboard.UIHelpers.Controls

open Avalonia
open Avalonia.Controls
open Avalonia.VisualTree
open ToolCupboard.UIHelpers.VisualTreeExtensions
open ToolCupboard.UIHelpers.Views

type Page() =
    inherit ContentControl()

    static member val TitleProperty = Window.TitleProperty.AddOwner<Page>()

    member this.Title
        with get () = this.GetValue(Page.TitleProperty)
        and set v = this.SetValue(Page.TitleProperty, v)

    member this.PageWindow with get () = this.Ancestor<PageWindow>()

    member this.PageNavigation with get () = this.PageWindow.PageNavigation

    member this.Navigate(page) =
        this.PageNavigation.Navigate(page)
