namespace ToolCupboard.Website.Models

open System.ComponentModel.DataAnnotations
open ToolCupboard.Database.Provider

type ToolViewModel() =
    [<Required>]
    member val Name = "" with get, set

    [<Required>]
    member val Location = "" with get, set

    [<Required>]
    member val Description = "" with get, set

    static member OfTool(tool:Tool) =
        ToolViewModel(
            Name = tool.Name, 
            Location = tool.Location, 
            Description = tool.Description)

    member this.UpdateTool(tool:Tool) =
        tool.Name <- this.Name
        tool.Location <- this.Location
        tool.Description <- this.Description
