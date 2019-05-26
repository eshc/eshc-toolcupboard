namespace ToolCupboard.Website.Models

open System
open System.ComponentModel.DataAnnotations
open ToolCupboard.Database.Provider

type EditUserCardViewModel() =
    member val CardId = "" with get, set

    member val Description = "" with get, set

    member val Added = DateTime.Now with get, set

    member val User = "" with get, set

type EditUserViewModel() =
    member val UserId = 0L with get, set

    [<Required>]
    member val Name = "" with get, set

    [<Required>]
    member val Username = "" with get, set

    [<Required>]
    member val Email = "" with get, set

    member val Cards : seq<EditUserCardViewModel> = Seq.empty with get, set

    static member OfUser (user : User) cards =
        EditUserViewModel(
            UserId = user.UserId,
            Name = user.Name, 
            Username = user.Login,
            Email = user.Email,
            Cards = cards)

    member this.UpdateUser(user : User) =
        user.Name <- this.Name
        user.Login <- this.Username
        user.Email <- this.Email

