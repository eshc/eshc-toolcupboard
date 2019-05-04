namespace ToolCupboard.Website.Models

open System.ComponentModel.DataAnnotations
open ToolCupboard.Database.Users

type ViewUsersUserViewModel(uid, username, name) =
    member val UserId : int64 = uid with get
    member val Username : string = username with get
    member val Name : string = name with get

type ViewUsersViewModel(users) =
    member val Users : seq<ViewUsersUserViewModel> = users with get


