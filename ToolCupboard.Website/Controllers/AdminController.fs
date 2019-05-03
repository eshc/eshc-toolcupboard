namespace ToolCupboard.Website.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Authorization
open ToolCupboard.Website.Models

[<Authorize(Policy = "AdminOnly")>]
type AdminController () =
    inherit Controller()

    member this.UnknownCards() =
        this.View()
