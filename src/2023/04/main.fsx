open System
open System.Collections.Generic

module Input =
  let private readLines () =
    stdin.ReadToEnd() |> (fun s -> s.Split "\n")

  let data = readLines ()

  let toStringArray (line: string) = line.ToCharArray() |> Array.map string

type ScratchCard =
  { id: int
    winningNumbers: Set<int>
    drawnNumbers: Set<int> }

let parseScratchCard id (winners: string) (drawnNumbers: string) =
  let toStringArray (line: string) =
    line.Trim().Split([| ' ' |])
    |> Array.choose (fun s ->
      s.Trim()
      |> function
        | "" -> None
        | x -> Some x)

  { id = id
    winningNumbers = winners |> toStringArray |> Array.map int |> Set.ofArray
    drawnNumbers = drawnNumbers |> toStringArray |> Array.map int |> Set.ofArray }

let parseScratchCards (lines: string[]) =
  lines
  |> Array.choose (fun line ->
    match line.Split([| ':'; '|' |]) with
    | [| id; winners; drawn |] ->
      let id = id.Split([| ' ' |]) |> Array.last |> int
      Some(parseScratchCard id winners drawn)
    | _ -> None)

let pow x y = Math.Pow(float x, float y) |> int

let playCard' card =
  card.winningNumbers |> Set.intersect card.drawnNumbers |> Set.count

let playCardDict = Dictionary()

let playCard card =
  match playCardDict.TryGetValue(card.id) with
  | true, count -> count
  | false, _ ->
    let count = playCard' card
    playCardDict.Add(card.id, count)
    count

let partOne cards =
  cards
  |> Array.sumBy (fun card -> card |> playCard |> (fun x -> if x > 0 then pow 2 (x - 1) else 0))

let partTwo cards =
  let cardCounts =
    cards
    |> Array.map(fun card -> card.id, 1)
    |> Map.ofArray
    |> Dictionary
  
  let rec inner i =
    match cards |> Array.tryItem i with
    | None -> cardCounts |> Seq.fold (fun sum kvp -> sum + kvp.Value) 0
    | Some card ->
      let numberOfCards = cardCounts[card.id]
      for i = 1 to numberOfCards do
        let count = playCard card
        if count = 0 then
          ()
        else
          let ids = [| card.id + 1 .. (card.id + count) |]
          for id in ids do
            cardCounts[id] <- cardCounts[id] + 1
        
      inner (i + 1)
  
  inner 0
        
let scratchCards = Input.data |> parseScratchCards

scratchCards |> partOne |> printfn "Part One: %i"

scratchCards |> partTwo |> printfn "Part Two: %i"
