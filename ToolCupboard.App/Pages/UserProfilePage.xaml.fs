namespace ToolCupboard.App.Pages

open System.Net.Http
open Avalonia.Controls
open Avalonia.Markup.Xaml
open Avalonia.Media.Imaging
open ToolCupboard.App
open ToolCupboard.App.ViewModels
open ToolCupboard.Common
open ToolCupboard.Database.Users
open ToolCupboard.Database.Tools
open ToolCupboard.UIHelpers.Controls
open System.IO
open System.Security.Cryptography
open System.Text

type UserProfilePage() as this =
    inherit Page()

    // do this.Initialized.AddHandler(fun o v -> this.OnInitialized(o,v))
    do this.InitializeComponent()

    member this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)

    member this.ViewModel with get () = this.DataContext :?> UserProfileViewModel

    member this.User with get () = this.ViewModel.User

    member this.LoadAvatarAsync() =
        async {
            try
                use client = new HttpClient()
                let data = MD5.Create().ComputeHash(Encoding.Default.GetBytes(this.User.Email)) |> Array.toList
                let md5 = Format.show Format.ppHexList data
                let url = sprintf "http://www.gravatar.com/avatar.php?gravatar_id=%s" md5
                let stream = client.GetByteArrayAsync(url)
                let! data = Async.AwaitTask(stream)
                let bmp = new Bitmap(new MemoryStream(data))
                printfn "data %A" bmp.Size
                this.FindControl<Image>("ProfilePictureImage").Source <- bmp
                //this.ViewModel.ProfilePicture <- bmp
                //this.InvalidateVisual()
            with e -> eprintfn "%A" e
        }

    member this.OnInitialized(sender, args) =
        Async.StartImmediate(this.LoadAvatarAsync())

    interface ICardHandler with
        member this.HandleUser user cardId popup =
            async {
                popup.SetText("Logging in...")

                if user.UserId = this.User.UserId then
                    this.PageControl |> Option.iter (fun pc -> 
                        pc.GoBack()
                        popup.SetText("Logged out.")
                    )
                else
                    (* get user data *)
                    let! tools = LookupUserToolsAsync None user
                    let dc = UserProfileViewModel(user, cardId, tools)

                    this.PageControl 
                    |> Option.iter (fun pc -> 
                        pc.Navigate(UserProfilePage(DataContext = dc), true)
                        popup.SetText(sprintf "Logged in as %s." user.Name)
                    )

                popup.StartFade()
            }

        member this.HandleTool tool cardId popup = 
            async {
                let! result = BorrowOrReturnToolAsync None tool this.User cardId this.ViewModel.CardId

                match result with
                | Borrowed -> 
                    this.ViewModel.Tools.Add(ToolViewModel(tool))
                    popup.SetText(sprintf "Borrowed %s." tool.Name)
                | Returned -> 
                    let remove = 
                        [ for t in this.ViewModel.Tools do
                            if (t.Tool.ToolId = tool.ToolId) then
                                yield t ]
                    this.ViewModel.Tools.RemoveAll(remove)
                    popup.SetText(sprintf "Returned %s." tool.Name)

                popup.StartFade()
            }

        member this.HandleUnknown cardId popup =
            async {
                popup.SetText(sprintf "Unknown card '%s'." cardId)
                popup.SetError()
                popup.StartFade()
            }