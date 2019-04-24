namespace ToolCupboard.App.Views

open System
open Avalonia.Controls
open Avalonia.Interactivity
open ToolCupboard.App
open ToolCupboard.UIHelpers.Controls

type DebugCardWindow =
    inherit Window
    new : mgr: Manager-> DebugCardWindow

    member InitializeComponent: unit -> unit

    member UserClick: sender: obj * args: RoutedEventArgs -> unit
