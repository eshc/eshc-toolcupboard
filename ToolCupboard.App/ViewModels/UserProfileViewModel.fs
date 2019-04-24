namespace ToolCupboard.App.ViewModels

open ToolCupboard.Database.Users
open ToolCupboard.Database.Tools
open ToolCupboard.UIHelpers.ViewModels

type UserProfileViewModel(user : User, tools : seq<Tool>) =
    inherit ViewModelBase()

    member val User = user

    member val Tools : AvaloniaList<ToolViewModel> = AvaloniaList()