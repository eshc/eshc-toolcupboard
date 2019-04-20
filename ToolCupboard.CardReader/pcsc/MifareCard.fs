namespace ToolCupboard.CardReader.PCSC

open PCSC
open PCSC.Iso7816
open ToolCupboard.Common

type MifareCard(reader : IIsoReader) =
    member this.FormatID(data) =
        Format.show Format.ppHexList (data |> Array.toList)

    member this.GetID() =
        let cmd = 
            CommandApdu(
                IsoCase.Case2Short, SCardProtocol.Any, 
                CLA = 0xFFuy, Instruction = InstructionCode.GetData,
                P1 = 0x00uy,
                P2 = 0x00uy,
                Le = 0)
        let response = reader.Transmit(cmd)
        let uid = response.GetData()
        this.FormatID(uid)

