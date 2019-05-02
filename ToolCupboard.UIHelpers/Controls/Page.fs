namespace ToolCupboard.UIHelpers.Controls

open Avalonia
open Avalonia.Controls
open Avalonia.VisualTree
open ToolCupboard.UIHelpers.VisualTreeExtensions

type Page() =
    inherit UserControl()

    static member val TitleProperty = Window.TitleProperty.AddOwner<Page>()

    member this.Title
        with get () = this.GetValue(Page.TitleProperty)
        and set v = this.SetValue(Page.TitleProperty, v)

    member this.PageControl with get () = this.Ancestor<PageControl>()

    member this.PageNavigation 
        with get () = 
            this.PageControl 
            |> Option.map (fun (v : PageControl) -> v.PageNavigation)

    member this.Navigate(page) =
        Option.iter (fun (v : PageNavigation) -> v.Navigate(page)) this.PageNavigation

