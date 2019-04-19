namespace UIHelpers

open System
open Avalonia
open Avalonia.Logging.Serilog
open System.Runtime.CompilerServices

[<Extension>]
module AppBuilderExtensions =
    [<Extension>]
    let UseUIHelpers(builder : 'a) = builder