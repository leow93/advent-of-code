open System.Text.Json.Nodes

let split (separator: string) (s: string) = s.Split separator

type Packet =
  | Int of int
  | Packets of Packet list

let rec parsePacket (json: JsonNode) =
  match json with
  | :? JsonValue as x -> Int(x.GetValue<int>())
  | :? JsonArray as x -> x |> Seq.map parsePacket |> List.ofSeq |> Packets
  | x -> failwithf "bad input: %A" x

let toTuple arr =
  arr |> Array.take 2 |> (fun xs -> (xs[0], xs[1]))

let parseFile file =
  System.IO.File.ReadAllText file
  |> split "\n\n"
  |> Array.map (
    split "\n"
    >> (Array.map (JsonNode.Parse >> parsePacket))
  )

let rec comparePackets a b =
  match a, b with
  | Int a, Int b when a < b -> -1
  | Int a, Int b when a > b -> 1
  | Int _, Int _ -> 0
  | Int _, Packets _ -> comparePackets (Packets [ a ]) b
  | Packets _, Int _ -> comparePackets a (Packets [ b ])
  | Packets a, Packets b ->
    (a, b)
    ||> Seq.fold2
          (fun acc a b ->
            match acc with
            | 0 -> comparePackets a b
            | x -> x)
          0
    |> (fun x ->
      match x with
      | 0 when a.Length < b.Length -> -1
      | 0 when a.Length > b.Length -> 1
      | x -> x)

let partOne file =
  parseFile file
  |> Array.map toTuple
  |> Array.mapi (fun i (a, b) ->
    match comparePackets a b with
    | -1 -> i + 1
    | _ -> 0)
  |> Array.sum
  |> (fun x -> printfn "Part I %s: %i" file x)

partOne "./test.txt"
partOne "./data.txt"

let makeDividerPacket = Int >> List.singleton >> Packets

let partTwo file =
  let dividerPacket2 = makeDividerPacket 2
  let dividerPacket6 = makeDividerPacket 6

  let sorted =
    parseFile file
    |> Array.concat
    |> Array.append [| dividerPacket2
                       dividerPacket6 |]
    |> Array.sortWith comparePackets

  let dp2Index =
    (sorted
     |> Array.findIndex (fun x -> x = dividerPacket2))
    + 1

  let dp6Index =
    (sorted
     |> Array.findIndex (fun x -> x = dividerPacket6))
    + 1

  dp2Index * dp6Index |> printfn "Part I %s: %i" file

partTwo "./test.txt"
partTwo "./data.txt"