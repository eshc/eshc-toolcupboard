namespace ToolCupboard.App.Pages

open Avalonia.Markup.Xaml
open ToolCupboard.App
open ToolCupboard.UIHelpers.Controls

type LockedPage() as this =
    inherit Page()

    do this.InitializeComponent()

    member this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)

    interface ICardHandler with
        member this.HandleUser user popup =
            this.PageControl.Navigate(UserProfilePage())

        member this.HandleTool card popup = ()
        member this.HandleUnknown card popup = ()