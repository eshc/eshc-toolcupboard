namespace ToolCupboard.Website.Models

open System.ComponentModel.DataAnnotations
open ToolCupboard.Database.Tools

type ViewToolsToolViewModel(toolId, name) =
    member val ToolId : int64 = toolId with get
    member val Name : string = name with get

type ViewToolsViewModel(tools) =
    member val Tools : seq<ViewToolsToolViewModel> = tools with get

