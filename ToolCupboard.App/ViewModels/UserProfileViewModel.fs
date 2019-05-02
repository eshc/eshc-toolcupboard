namespace ToolCupboard.App.ViewModels

open Avalonia.Collections
open ToolCupboard.Database.Users
open ToolCupboard.Database.Tools
open ToolCupboard.UIHelpers.ViewModels

type UserProfileViewModel(user : User, cardId : string, tools : seq<Tool>) =
    inherit ViewModelBase()

    member val CardId = cardId

    member val User = user

    member this.Name with get () = this.User.Name
    
    member this.Email with get () = this.User.Email

    member val Tools : AvaloniaList<ToolViewModel> = AvaloniaList(Seq.map ToolViewModel tools)