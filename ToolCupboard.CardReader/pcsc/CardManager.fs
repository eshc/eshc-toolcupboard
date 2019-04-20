namespace ToolCupboard.CardReader.PCSC

open PCSC
open PCSC.Iso7816
open ToolCupboard.CardReader

type CardManager() =
    member val private Context : ISCardContext = null with get,set

    member val private Monitors = [] with get,set

    member val private LastCardId = None with get,set

    interface ICardManager with
        member val CardInserted = new Event<_>()
        member val CardRemoved = new Event<_>()

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
        use isoReader = 
            new IsoReader(this.Context,  args.ReaderName, SCardShareMode.Shared, SCardProtocol.Any, false)
        let card = MifareCard(isoReader)
        let cardid = card.GetID()
        (this :> ICardManager).CardInserted.Trigger(this :> obj, CardEventArgs(CardId = cardid))
        this.LastCardId <- Some cardid

    member this.OnCardRemoved(args : Monitoring.CardEventArgs) =
        this.LastCardId
        |> Option.iter (fun cardid -> 
            (this :> ICardManager).CardRemoved.Trigger(this :> obj, CardEventArgs(CardId = cardid))
        )




