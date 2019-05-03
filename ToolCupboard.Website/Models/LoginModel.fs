namespace ToolCupboard.Website.Models

open System.ComponentModel.DataAnnotations

type LoginModel() =
    [<Required>]
    member val Username = "" with get, set

    [<Required>][<DataType(DataType.Password)>]
    member val Password = "" with get, set