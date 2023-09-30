open System
open System.Collections.Generic

module Input =
  let realData =
    System.IO.File.ReadAllLines "./data.txt"

  let testData =
    System.IO.File.ReadAllLines "./test.txt"

  let parse (lines: string []) =
    let initial = lines |> Array.head
    let rest = lines |> Array.tail

    let rules =
      rest
      |> Array.filter (not << String.IsNullOrEmpty)
      |> Array.choose (fun rule ->
        match rule.Split " -> " with
        | [| a; b |] -> Some(a, b)
        | _ -> None)
      |> Map.ofArray

    initial, rules

let evolve rules (s: string) =
  let stringArr =
    s.ToCharArray() |> Array.map string

  let rec loop acc idx =
    match stringArr |> Array.tryItem idx, stringArr |> Array.tryItem (idx + 1) with
    | Some a, Some b ->
      // printfn "%s, %s, %s" acc a b
      match rules |> Map.tryFind (a + b) with
      | Some x -> loop (acc + a + x) (idx + 1)
      | None -> loop (acc + a + b) (idx + 1)
    | Some a, None -> acc + a
    | _ -> acc


  loop "" 0

let partOne steps input =
  let initial, rules = input |> Input.parse

  let m =
    initial
    |> Seq.pairwise
    |> Seq.countBy id
    |> Seq.map (fun (key, count) -> key, uint64 count)
    |> Map


  { 1..steps
  }
  |> Seq.fold (fun (acc: string) _ -> evolve rules acc) initial
  |> fun s -> s.ToCharArray()
  |> Array.countBy id
  |> (fun arr ->
    (arr |> Array.maxBy snd |> snd)
    - (arr |> Array.minBy snd |> snd))

Input.testData
|> partOne 10
|> printfn "Part I (test): %i"

Input.realData
|> partOne 10
|> printfn "Part I (actual): %i"

// Input.testData
// |> partOne 40
// |> printfn "Part II (test): %i"

// Input.realData
// |> partOne 40
// |> printfn "Part II (actual): %i"

let inline charsToKey a b = sprintf "%c%c" a b

type CountMap<'K when 'K: comparison> = Map<'K, int64>

module PartTwo =

  let increment<'Key when 'Key: comparison> (key: 'Key) (by: int64) (cm: CountMap<'Key>) =
    let value =
      cm.TryFind key |> Option.defaultValue 0L

    cm.Add(key, (value + by))

  let private applyPairs (rules: Map<string, string * string * char>) (polymerCount, pairCount) (kv: KeyValuePair<string, int64>) =
    let pair = kv.Key
    let value = kv.Value
    let (pair1, pair2, p) = rules[pair]

    (polymerCount |> increment p value,
     pairCount
     |> increment pair1 value
     |> increment pair2 value)

  let rec private apply n rules polymers pairs =

    match n with
    | 0 -> (polymers, pairs)
    | _ ->
      let polC, pairC =
        pairs
        |> Seq.fold (applyPairs rules) (polymers, Map.empty)

      apply (n - 1) rules polC pairC

  let solve n input =
    let polymer, rules = input |> Input.parse

    let polymers =
      polymer
      |> Seq.countBy id
      |> Seq.map (fun (key, count) -> key, int64 count)
      |> Map

    let pairs =
      polymer
      |> Seq.pairwise
      |> Seq.countBy id
      |> Seq.map (fun (key, count) -> (key ||> charsToKey), int64 count)
      |> Map

    let rules' =
      input
      |> Array.skip 2
      |> Array.map (fun l -> charsToKey l[0] l[1], (charsToKey l[0] l[6], charsToKey l[6] l[1], l[6]))
      |> Map

    let finalComposition, _ =
      apply n rules' polymers pairs

    let counts =
      finalComposition |> Seq.map (fun kv -> kv.Value)

    let mostCommon = counts |> Seq.max
    let leastCommon = counts |> Seq.min
    mostCommon - leastCommon

Input.realData
|> PartTwo.solve 10
|> printfn "Part I (actual): %i"

Input.realData
|> PartTwo.solve 40
|> printfn "Part II (actual): %i"
