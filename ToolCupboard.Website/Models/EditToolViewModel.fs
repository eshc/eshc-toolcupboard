namespace ToolCupboard.Website.Models

open System.ComponentModel.DataAnnotations
open ToolCupboard.Database.Provider
open System

type EditToolCardViewModel() =
    member val CardId = "" with get, set

    member val Description = "" with get, set

    member val Added = DateTime.Now with get, set

    member val Tool = 0L with get, set

type EditToolViewModel() =
    member val ToolId = 0L with get, set

    [<Required>]
    member val Name = "" with get, set

    [<Required>]
    member val Location = "" with get, set

    [<Required>]
    member val Description = "" with get, set

    member val LastUnknownCard = "" with get, set

    member val BorrowedBy = "" with get, set

    member val Cards : seq<EditToolCardViewModel> = Seq.empty with get, set

    static member OfTool(tool:Tool) cards =
        EditToolViewModel(
            ToolId = tool.ToolId,
            Name = tool.Name, 
            Location = tool.Location, 
            Description = tool.Description,
            Cards = cards)

    member this.UpdateTool(tool:Tool) =
        tool.Name <- this.Name
        tool.Location <- this.Location
        tool.Description <- this.Description
