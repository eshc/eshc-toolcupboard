namespace ToolCupboard.Website.Models

open System.ComponentModel.DataAnnotations
open ToolCupboard.Database.Tools

type ViewToolsToolViewModel(toolId, name, borrower) =
    member val ToolId : int64 = toolId with get
    member val Name : string = name with get
    member val BorrowedBy : string = borrower with get

type ViewToolsViewModel(tools) =
    member val Tools : seq<ViewToolsToolViewModel> = tools with get

