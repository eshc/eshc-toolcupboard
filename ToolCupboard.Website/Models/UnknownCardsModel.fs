namespace ToolCupboard.Website.Models
open System

type UnknownCardModel() =
    member val CardId = "" with get, set
    member val LastDate = DateTime.Now with get, set

type UnknownCardsModel(cards : seq<UnknownCardModel>) =
    member val Cards = cards with get, set

