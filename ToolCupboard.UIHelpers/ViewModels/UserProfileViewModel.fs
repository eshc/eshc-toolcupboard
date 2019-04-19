namespace ToolCupboard.UIHelpers.ViewModels

open Avalonia.Collections

type UserProfileViewModel() =

    member val Name = "" with get, set

    member val Tools : AvaloniaList<ToolViewModel> = AvaloniaList() with get, set