open System
open System.Collections.Generic

module Input =
  let private readLines () =
    stdin.ReadToEnd() |> (fun s -> s.Split "\n")

  let data = readLines ()

type ScratchCard =
  { id: int
    winningNumbers: Set<int>
    drawnNumbers: Set<int> }

module ScratchCard =
  let private parse id (winners: string) (drawnNumbers: string) =
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

  let fromLine (line: string) : ScratchCard option =
    match line.Split([| ':'; '|' |]) with
    | [| id; winners; drawn |] ->
      let id = id.Split([| ' ' |]) |> Array.last |> int
      Some(parse id winners drawn)
    | _ -> None

let pow x y = Math.Pow(float x, float y) |> int
let pow2 = pow 2

type PlayCard() =
  let dict = Dictionary<int, int>()

  member this.Play card =
    if dict.ContainsKey(card.id) then
      dict[card.id]
    else
      let x = card.winningNumbers |> Set.intersect card.drawnNumbers |> Set.count
      dict.Add(card.id, x)
      x

let cardPlayer = PlayCard()
let playCard = cardPlayer.Play

let partOne cards =
  cards
  |> Array.sumBy (fun card ->
    card
    |> playCard
    |> (function
    | 0 -> 0
    | x -> pow2 (x - 1)))

let partTwo cards =
  let cardCounts =
    cards |> Array.map (fun card -> card.id, 1) |> Map.ofArray |> Dictionary

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

let scratchCards = Input.data |> Array.choose ScratchCard.fromLine

scratchCards |> partOne |> printfn "Part One: %i"
scratchCards |> partTwo |> printfn "Part Two: %i"
