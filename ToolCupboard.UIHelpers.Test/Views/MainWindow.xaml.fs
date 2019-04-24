namespace ToolCupboard.UIHelpers.Test.Views

open Avalonia
open Avalonia.Collections
open Avalonia.Controls
open Avalonia.Interactivity
open Avalonia.Markup.Xaml
open ToolCupboard.UIHelpers.Controls
open ToolCupboard.UIHelpers.ViewModels
open ToolCupboard.UIHelpers.Pages
open ToolCupboard.UIHelpers.Views

type MainWindow () as this =
    inherit Window()

    let lastWindow = PageWindow(Width = 480.0, Height = 800.0)

    do this.InitializeComponent()
    member this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)
        lastWindow.InitializeComponent()
        lastWindow.Show()

    member this.UserProfileClick(sender: obj, ev : RoutedEventArgs) =
        // let tools = AvaloniaList([ToolViewModel(Name = "Some Tool")])
        // let vm = UserProfileViewModel(Name = "Rudi", Tools = tools)
        // let page = UserProfilePage(DataContext = vm)
        // lastWindow.Navigate(page)
        ()

    member this.LockedClick(sender: obj, ev : RoutedEventArgs) =
        // let page = LockedPage()
        // lastWindow.Navigate(page)
        ()

    member this.SendMessageClick(sender: obj, ev : RoutedEventArgs) =
        lastWindow.PopupMessageBox("Hello world!")
