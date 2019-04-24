namespace ToolCupboard.App.Pages

open Avalonia
open Avalonia.Controls
open Avalonia.Markup.Xaml
open ToolCupboard.UIHelpers.Controls

type UserProfilePage() as this =
    inherit Page()

    do this.InitializeComponent()

    member this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)
