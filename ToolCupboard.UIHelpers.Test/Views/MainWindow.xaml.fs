namespace ToolCupboard.UIHelpers.Test.Views

open Avalonia
open Avalonia.Collections
open Avalonia.Controls
open Avalonia.Interactivity
open Avalonia.Markup.Xaml
open ToolCupboard.UIHelpers.Controls
open ToolCupboard.UIHelpers.ViewModels
open ToolCupboard.UIHelpers.Views

type MainWindow () as this =
    inherit Window()

    let lastWindow = ref None

    do this.InitializeComponent()
    member this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)
    

    member this.UserProfileClick(sender: obj, ev : RoutedEventArgs) =
        let tools = AvaloniaList([ToolViewModel(Name = "Some Tool")])
        let vm = UserProfileViewModel(Name = "Rudi", Tools = tools)
        let wnd = UserProfileWindow(DataContext = vm, Width = 480.0, Height = 800.0)
        lastWindow := Some (wnd :> ToolWindow)
        wnd.Show()

    member this.LockedClick(sender: obj, ev : RoutedEventArgs) =
        let wnd = LockedWindow(Width = 480.0, Height = 800.0)
        lastWindow := Some (wnd :> ToolWindow)
        wnd.Show()

    member this.SendMessageClick(sender: obj, ev : RoutedEventArgs) =
        !lastWindow
        |> Option.iter (fun wnd ->
            wnd.PopupMessageBox("hello world")
        ) 