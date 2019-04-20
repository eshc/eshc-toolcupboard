namespace ToolCupboard.UIHelpers.Controls

open Avalonia.Controls

type PageNavigation() =
    member val Pages = [] with get, set

    member this.Navigate(window : ContentControl, page) =
        this.Pages <- page :: this.Pages
        window.Content <- page

    member this.GoBack(window : ContentControl) =
        this.Pages <- List.tail this.Pages
        window.Content <- List.head this.Pages

    member this.CanGoBack with get () = List.isEmpty this.Pages |> not
