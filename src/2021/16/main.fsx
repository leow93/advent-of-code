open System

type Operator =
  | Sum = 0
  | Product = 1
  | Min = 2
  | Max = 3
  | Literal = 4
  | GreaterThan = 5
  | LessThan = 6
  | EqualTo = 7

type LengthType =
  | LengthInBits = 0
  | NumberOfSubPackets = 1

type Packet =
  | Literal of int * int64
  | Operation of int * Operator * Packet list

let hexToDecimal (c: char) = Convert.ToInt32(string c, 16)

let decimalToBinary (d: int) = Convert.ToString(d, 2).PadLeft(4, '0')

let hexToBinary =
  hexToDecimal >> decimalToBinary

let binaryToNumber (s: string) f l = Convert.ToInt32(s[f .. (f + l - 1)], 2)

let parseOperatorType s = binaryToNumber s 3 3 |> enum<Operator>

let part (s: string) f l = s.[f .. (f + l - 1)]

let parseLiteralValue (data: string) =
  let rec parseLiteralValueRec value consumed n =
    let next = part data (5 * n) 5
    let nextValue = value + part next 1 4

    match next.[0] = '1' with
    | true -> parseLiteralValueRec nextValue (consumed + 5) (n + 1)
    | false -> Convert.ToInt64(nextValue, 2), consumed + 5

  parseLiteralValueRec "" 0 0

let parseLiteral (data: string) =
  let version = binaryToNumber data 0 3
  let remaining = data.[6..]

  let (value, consumed) =
    parseLiteralValue remaining

  (Literal(version, value), data.[6 + consumed ..])

let rec parsePacket (data: string) =
  match data with
  | "" -> None
  | _ ->
    let operatorType = parseOperatorType data

    match operatorType with
    | Operator.Literal -> Some(parseLiteral data)
    | _ ->
      let version = binaryToNumber data 0 3

      let lengthType =
        binaryToNumber data 6 1 |> enum<LengthType>

      match lengthType with
      | LengthType.LengthInBits ->
        let totalLengthInBits =
          binaryToNumber data 7 15

        let subPacketsData =
          part data 22 totalLengthInBits

        let subPackets =
          List.unfold parsePacket subPacketsData

        Some(Operation(version, operatorType, subPackets), data.[22 + totalLengthInBits ..])
      | LengthType.NumberOfSubPackets ->
        let numberOfSubPackets =
          binaryToNumber data 7 11

        let subPacketsData = data.[18..]

        let subPackets, remaining =
          [ 1..numberOfSubPackets ]
          |> List.fold
               (fun (ps, sd) _ ->
                 // trusting input here
                 // and hoping Option.get
                 // will not throw error
                 let p, r = parsePacket sd |> Option.get
                 (p :: ps, r))
               ([], subPacketsData)
        // need List.rev as fold is adding p at the top
        // correct order needed for GreaterThan, LessThan
        Some(Operation(version, operatorType, List.rev subPackets), remaining)
      | _ -> None

let rec versionSum (packet: Packet) =
  match packet with
  | Literal (v, _) -> v
  | Operation (v, _, sp) -> v + List.sumBy versionSum sp

let rec evaluate (packet: Packet) =
  match packet with
  | Literal (_, v) -> v
  | Operation(_, op, sp) ->
    let values = sp |> List.map evaluate
    
    match op, values with
    | Operator.Sum, l -> l |> List.sum
    | Operator.Product, l -> l |> List.reduce (*)
    | Operator.Min, l -> List.min l
    | Operator.Max, l -> List.max l
    | Operator.GreaterThan, [ a; b ] -> if a > b then 1L else 0L
    | Operator.LessThan, [ a; b ] -> if a < b then 1L else 0L
    | Operator.EqualTo, [ a; b ] -> if a = b then 1L else 0L
    | _ -> failwith "Invalid operation"



let main file =
  let x =
    file
    |> System.IO.File.ReadAllText
    |> Seq.map hexToBinary
    |> String.Concat
    |> parsePacket
  
  match x with
  | None -> 0, 0L
  | Some vr ->
    let packet = fst vr
    versionSum packet, evaluate packet

"data.txt" |> main |> printfn "Part I, II: %A"
