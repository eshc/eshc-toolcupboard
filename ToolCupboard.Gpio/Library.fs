namespace ToolCupboard.Gpio

open System.Device.Gpio
open System.Threading.Tasks
open System
open System.Device.Gpio
open System.Device.Gpio

type GpioManager() =
    let reed = 18
    let lock = 17
    let ctrl = new GpioController()
    let doorOpened = new Event<_>()
    let doorClosed = new Event<_>()

    member this.DoorOpened = doorOpened.Publish
    member this.DoorClosed = doorClosed.Publish

    member this.Initialize() =
        ctrl.OpenPin(reed, PinMode.InputPullUp)
        ctrl.OpenPin(lock, PinMode.Input)

        ctrl.RegisterCallbackForPinValueChangedEvent(
            reed, 
            PinEventTypes.Falling ||| PinEventTypes.Rising, 
            PinChangeEventHandler(this.ReedCallback))

    member private this.ReedCallback sender (args : PinValueChangedEventArgs) =
        match args.ChangeType with
        | PinEventTypes.Falling -> doorClosed.Trigger(this :> obj)
        | PinEventTypes.Rising -> doorOpened.Trigger(this :> obj)
        | _ -> ()

    member private this.Unlock() =
        ctrl.SetPinMode(lock, PinMode.Output)
        ctrl.Write(lock, PinValue.Low)

    member private this.Lock() =
        ctrl.SetPinMode(lock, PinMode.Input)

    member this.OpenDoor() = 
        async {
            this.Unlock()
            do! Task.Delay(TimeSpan.FromSeconds(10.0)) |> Async.AwaitTask
            this.Lock()
        }

