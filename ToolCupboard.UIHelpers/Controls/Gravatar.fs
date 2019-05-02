namespace ToolCupboard.UIHelpers.Controls

open Avalonia
open Avalonia.Controls
open Avalonia.Media.Imaging
open ToolCupboard.Common
open System.Net.Http
open System.Security.Cryptography
open System.IO
open System.Text
open System

type Gravatar() =
    inherit Image()

    static let emailChanged (o : IAvaloniaObject) (v : bool) =
        if v then
            let obj = o :?> Gravatar
            Async.StartImmediate(obj.LoadAvatarAsync())

    static member val EmailProperty = AvaloniaProperty.Register<Gravatar, string>("Email", notifying = Action<_,_>(emailChanged))

    member this.Email
        with get () = this.GetValue(Gravatar.EmailProperty)
        and set v = this.SetValue(Gravatar.EmailProperty, v)

    member this.LoadAvatarAsync() =
        async {
            try
                let size = int this.Height |> (fun v -> if v = 0 then 80 else v)
                use client = new HttpClient()
                let data = MD5.Create().ComputeHash(Encoding.Default.GetBytes(this.Email)) |> Array.toList
                let md5 = Format.show Format.ppHexList data
                let url = sprintf "https://www.gravatar.com/avatar/%s?s=%d&d=retro&r=x" md5 size
                let stream = client.GetByteArrayAsync(url)
                let! data = Async.AwaitTask(stream)
                let bmp = new Bitmap(new MemoryStream(data))
                this.Source <- bmp
            with e -> eprintfn "%A" e
        }