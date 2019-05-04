module ToolCupboard.Common.Result
open System

let map = Result.map
let bind = Result.bind
let result v = Result.Ok v
let error v = Result.Error v
let tryWith f err = try f () |> result with _ -> err () |> error
let tryErr f err = try f () |> result with _ -> err |> error
let ofBool b err = if b then result () else error err
let combine expr1 expr2 = 
    expr1 |> bind (fun () -> expr2)

let (>>=) v f = bind

type ResultBuilder() =
    member x.Combine(expr1, expr2) = combine expr1 expr2
    member x.Bind(comp, func) = bind func comp
    member x.Return(value) = result value
    member x.Zero() = result ()
    member x.Delay(f) = f ()
    member x.Using(resource : #IDisposable, expr) = try expr resource finally resource.Dispose()
