namespace ToolCupboard.UIHelpers.Views

open Avalonia
open Avalonia.Controls
open Avalonia.Markup.Xaml
open ToolCupboard.UIHelpers.Controls

type UserProfileWindow () as this =
    inherit ToolWindow()

    do this.InitializeComponent()

    member this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)