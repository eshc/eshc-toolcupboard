namespace ToolCupboard.App.Views

open Avalonia.Controls
open Avalonia.Interactivity
open Avalonia.Markup.Xaml
open ToolCupboard.App
open ToolCupboard.CardReader.Debug

type DebugCardWindow (mgr: Manager) as this =
    inherit Window()

    do this.InitializeComponent()
    member this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)

    member this.UserClick(sender : obj, ev : RoutedEventArgs) =
        let btn = sender :?> Button
        let number = btn.Content :?> string
        (mgr.CardManager :?> CardManager).TriggerCardInserted("100" + number)

    member this.ToolClick(sender : obj, ev : RoutedEventArgs) =
        let btn = sender :?> Button
        let number = btn.Content :?> string
        (mgr.CardManager :?> CardManager).TriggerCardInserted("200" + number)
