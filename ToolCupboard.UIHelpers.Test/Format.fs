module UIHelpersTest.Format

open System.Text
open System.IO

let ppList sep f b l =
    List.iteri (fun i v ->
        if i > 0 then sep b ()
        f b v
    ) l

let ppEmpty _ () = ()

let ppComma b () = fprintf b ", "

let ppHexByte b (v : byte) = fprintf b "%02X" v

let ppHexList b v = (ppList ppEmpty ppHexByte) b v

let show fmt v =
    let sb = StringBuilder()
    use f = new StringWriter(sb)
    Printf.fprintf f "%a" fmt v 
    f.Flush()
    sb.ToString()
