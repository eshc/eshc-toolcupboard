namespace ToolCupboard.UIHelpers.Views

open Avalonia
open Avalonia.Collections
open Avalonia.Controls
open Avalonia.Interactivity
open Avalonia.Markup.Xaml
open ToolCupboard.UIHelpers.Controls
open ToolCupboard.UIHelpers.ViewModels

type PageWindow () as this =
    inherit Window()

    do this.InitializeComponent()
    member this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)

    member this.PageControl
        with get () = this.FindControl<PageControl>("PageControl")

    member this.Navigate(target) =
        this.PageControl.Navigate(target)

    member this.PopupMessageBox(msg) =
        this.PageControl.PopupMessageBox(msg)
