namespace ToolCupboard.Website.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Authorization
open ToolCupboard.Database
open ToolCupboard.Website.Models

[<Authorize(Policy = "AdminOnly")>]
type AdminController () =
    inherit Controller()

    member this.UnknownCards() =
        async {
            let! cards = AccessLog.LastUnknownCards None 50
            let model = UnknownCardsModel(cards |> Seq.map (fun v -> UnknownCardModel(CardId = v.CardId, LastDate = v.LastDate)))
            return this.View(model)
        }
