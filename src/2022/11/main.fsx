open System.Collections.Generic

let read = System.IO.File.ReadAllText
let split (sep: string) (s: string) = s.Split(sep)

type Monkey =
  { id: int
    items: ResizeArray<int>
    divisibleTest: int
    trueCase: int
    falseCase: int
    operation: int -> int }

module Operation =
  let add a b = a + b
  let multiply a b = a * b
  let square a = a * a

  let parse (s: string) =
    match split " = " s with
    | [| _; s |] ->
      match split " " s with
      | [| "old"; "*"; "old" |] -> square
      | [| "old"; "*"; x |] -> multiply (int x)
      | [| "old"; "+"; x |] -> add (int x)
      | _ -> failwithf "Could not parse operation: %s" s
    | _ -> failwithf "Could not parse operation: %s" s

module Monkey =
  let private parseMonkeyId (s: string) =
    if s.StartsWith "Monkey " then
      match split ":" (s.Substring 7) with
      | [| x; _ |] -> int x
      | _ -> failwithf "Could not parse monkey id: %s" s
    else
      failwithf "Could not parse monkey id: %s" s

  let private parseStartingItems (s: string) =
    let s = s.Trim()
    let result = ResizeArray()

    if s.StartsWith("Starting items: ") then
      s.Substring 16
      |> split ","
      |> Array.iter (fun x -> result.Add(int x))

    result

  let private parseTest (s: string) =
    match split " " s |> Array.tryLast with
    | Some x -> int x
    | None -> failwithf "Could not parse test %s" s

  let private parseTrueCase (s: string) =
    match split " " s |> Array.tryLast with
    | Some x -> int x
    | None -> failwithf "Could not parse trueCase %s" s

  let private parseFalseCase (s: string) =
    match split " " s |> Array.tryLast with
    | Some x -> int x
    | None -> failwithf "Could not parse falseCase %s" s

  let parse s =
    match split "\n" s with
    | [| monkeyId; startingItems; operation; test; trueCase; falseCase |] ->
      { id = parseMonkeyId monkeyId
        items = parseStartingItems startingItems
        divisibleTest = parseTest test
        operation = Operation.parse operation
        trueCase = parseTrueCase trueCase
        falseCase = parseFalseCase falseCase }
    | _ -> failwithf "unknown monkey: %s" s

let parse text =
  text |> split "\n\n" |> Array.map Monkey.parse

let turn (monkeys: Monkey []) (monkey: Monkey) =
  monkey.items.ForEach (fun item ->
    let worryLevel =
      floor (float (monkey.operation item) / float 3)
      |> int

    let throwToMonkey =
      match worryLevel % monkey.divisibleTest = 0 with
      | true -> monkey.trueCase
      | false -> monkey.falseCase

    monkeys[ throwToMonkey ].items.Add worryLevel)

  let inspectionCount = monkey.items.Count
  monkey.items.Clear()
  monkey, inspectionCount

let toArray (rArray: ResizeArray<'a>) =
  let mutable result = Array.empty

  for x in rArray do
    result <- Array.append result [| x |]

  result

let round monkeys =
  let takeTurn = turn monkeys
  monkeys |> Array.map takeTurn

let partOne file =
  let mutable monkeys = read file |> parse
  let counts = Dictionary()

  for _ in 1..20 do
    let monkeysWithCounts = round monkeys

    monkeysWithCounts |> Array.map (fun (m, count) -> m.id, count) |> Array.iter(fun (id, count) ->
      match counts.TryGetValue id with
      | true, v ->
        counts[id] <- v + count
      | false, _ ->
        counts.Add(id, count)
      )
    monkeys <- monkeysWithCounts |> Array.map fst
  
  counts
  |> Dictionary.ValueCollection
  |> Seq.sortDescending
  |> Seq.take 2
  |> Seq.fold (fun a b -> a * b) 1

partOne "./test.txt"
|> printfn "Part I (TEST): %i"

partOne "./data.txt"
|> printfn "Part I (REAL): %i"

