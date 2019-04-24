namespace ToolCupboard.UIHelpers.Controls

open Avalonia.Controls

type PageNavigation() =
    member val Pages = [] with get, set

    member this.Navigate(cc : ContentControl, page) =
        this.Pages <- page :: this.Pages
        cc.Content <- page

    member this.GoBack(cc : ContentControl) =
        this.Pages <- List.tail this.Pages
        cc.Content <- List.head this.Pages

    member this.CanGoBack with get () = List.isEmpty this.Pages |> not
