namespace ToolCupboard.App.Pages

open Avalonia.Markup.Xaml
open ToolCupboard.App
open ToolCupboard.App.ViewModels
open ToolCupboard.Database.Users
open ToolCupboard.UIHelpers.Controls

type LockedPage() as this =
    inherit Page()

    do this.InitializeComponent()

    member this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)

    interface ICardHandler with
        member this.HandleUser mgr user cardId popup =
            async {
                popup.SetText("Logging in...")

                let! tools = LookupUserToolsAsync None user

                let dc = UserProfileViewModel(user, cardId, tools)
                match this.PageControl with
                | Some pc ->
                    printfn"Unlocking door"
                    mgr.OpenDoor() |> fun v -> Async.Start(v)
                    pc.Navigate(UserProfilePage(DataContext = dc))
                    popup.SetText(sprintf "Logged in as %s." user.Name)
                | None -> ()

                popup.StartFade()
            }

        member this.HandleTool mgr tool cardId popup = 
            async {
                popup.SetText("Not logged in yet.")
                popup.SetError()
                popup.StartFade()
            }

        member this.HandleUnknown mgr cardId popup =
            async {
                popup.SetText(sprintf "Unknown card '%s'." cardId)
                popup.SetError()
                popup.StartFade()
            }

        member this.HandleDoorClosed mgr =
            async {
                do ()
            }

        member this.HandleDoorOpened mgr =
            async {
                do ()
            }