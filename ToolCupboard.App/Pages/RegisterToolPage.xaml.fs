namespace ToolCupboard.App.Pages

open Avalonia.Markup.Xaml
open ToolCupboard.App
open ToolCupboard.App.ViewModels
open ToolCupboard.Database.Users
open ToolCupboard.Database.Tools
open ToolCupboard.UIHelpers.Controls

type RegisterToolPage() as this =
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
                this.PageControl |> Option.iter (fun pc ->
                    pc.GoBack()
                    pc.Navigate(UserProfilePage(DataContext = dc), true)
                )

                popup.SetText(sprintf "Logged in as %s." user.Name)
                popup.StartFade()
            }

        member this.HandleTool mgr tool cardId popup = 
            async {
                popup.SetText("This tool is already registered.")
                popup.SetError()
                popup.StartFade()
            }

        member this.HandleUnknown mgr cardId popup =
            async {
                let! tool = RegisterToolAsync None cardId
                popup.SetText(sprintf "Registered tool %d." tool.ToolId)
                popup.SetError()
                popup.StartFade()
                this.PageControl |> Option.iter (fun pc  -> pc.GoBack())
            }

        member this.HandleDoorClosed mgr =
            async {
                do ()
            }

        member this.HandleDoorOpened mgr =
            async {
                do ()
            }