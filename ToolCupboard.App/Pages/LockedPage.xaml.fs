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
        member this.HandleUser user cardId popup =
            async {
                popup.SetText("Logging in...")

                let! tools = LookupUserToolsAsync None user

                let dc = UserProfileViewModel(user, cardId, tools)
                Option.iter (fun (pc : PageControl) -> 
                    pc.Navigate(UserProfilePage(DataContext = dc))
                    popup.SetText(sprintf "Logged in as %s." user.Name)
                ) this.PageControl

                popup.StartFade()
            }

        member this.HandleTool tool cardId popup = 
            async {
                popup.SetText("Not logged in yet.")
                popup.SetError()
                popup.StartFade()
            }

        member this.HandleUnknown cardId popup =
            async {
                popup.SetText(sprintf "Unknown card '%s'." cardId)
                popup.SetError()
                popup.StartFade()
            }