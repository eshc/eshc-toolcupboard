namespace ToolCupboard.App

open Avalonia
open Avalonia.Markup.Xaml
open ToolCupboard.CardReader

type App() =
    inherit Application()

    member this.CardManager =
        ToolCupboard.CardReader.PCSC.CardManager() :> ICardManager

    override this.Initialize() =
        this.CardManager.Initialize()
        AvaloniaXamlLoader.Load(this)
