namespace ToolCupboard.CardReader.PCSC

open PCSC
open PCSC.Iso7816
open ToolCupboard.CardReader

type CardManager() =
    let cardInserted = new Event<_>()
    let cardRemoved = new Event<_>()

    member val private Context : ISCardContext = null with get,set

    member val private Monitors = [] with get,set

    member val private LastCardId = None with get,set

    interface ICardManager with
        [<CLIEvent>]
        member this.CardInserted = cardInserted.Publish

        [<CLIEvent>]
        member this.CardRemoved = cardRemoved.Publish

        member this.Initialize() = 
            this.Context <- ContextFactory.Instance.Establish(SCardScope.System)

            this.Monitors <- 
                this.Context.GetReaders()
                |> List.ofArray
                |> List.map (fun reader -> 
                    printfn "Initializing card monitor %s..." reader
                    let monFactory = PCSC.Monitoring.MonitorFactory.Instance
                    let mon = monFactory.Create(SCardScope.System)
                    mon.CardInserted.Add(this.OnCardInserted)
                    mon.CardRemoved.Add(this.OnCardRemoved)
                    mon.Start(reader)
                    mon)

    member this.OnCardInserted(args : Monitoring.CardEventArgs) = 
        try
            use isoReader = 
                new IsoReader(this.Context,  args.ReaderName, SCardShareMode.Shared, SCardProtocol.Any, false)
            let card = MifareCard(isoReader)
            let cardid = card.GetID()
            cardInserted.Trigger(this :> obj, CardEventArgs(CardId = cardid))
            this.LastCardId <- Some cardid
        with _ ->
            ()

    member this.OnCardRemoved(args : Monitoring.CardEventArgs) =
        try
            this.LastCardId
            |> Option.iter (fun cardid -> 
                cardRemoved.Trigger(this :> obj, CardEventArgs(CardId = cardid))
            )
        with _ ->
            ()




