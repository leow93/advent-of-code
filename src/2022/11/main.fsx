let read = System.IO.File.ReadAllText
let split (sep: string) (s: string) = s.Split(sep)

type Monkey =
  { id: int
    items: uint64 list
    divisibleTest: uint64
    trueCase: int
    falseCase: int
    operation: uint64 -> uint64
    inspectionCount: uint64 }

module Operation =
  let add a b = a + b
  let multiply a b = a * b
  let square a = a * a

  let parse (s: string) =
    match split " = " s with
    | [| _; s |] ->
      match split " " s with
      | [| "old"; "*"; "old" |] -> square
      | [| "old"; "*"; x |] -> multiply (uint64 x)
      | [| "old"; "+"; x |] -> add (uint64 x)
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

    if s.StartsWith("Starting items: ") then
      s.Substring 16
      |> split ","
      |> Array.map uint64
      |> List.ofArray
    else
      []

  let private parseTest (s: string) =
    match split " " s |> Array.tryLast with
    | Some x -> uint64 x
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
        falseCase = parseFalseCase falseCase
        inspectionCount = 0UL }
    | _ -> failwithf "unknown monkey: %s" s

let parse text =
  text |> split "\n\n" |> Array.map Monkey.parse

type PlayRounds(n: int, worryManager: uint64 -> uint64, monkeys: Map<int, Monkey>) =

  let evolve monkeys monkey =
    let inspectionCount =
      monkey.items.Length |> uint64

    let mutable monkeys = monkeys

    for item in monkey.items do
      let worryLevel =
        item |> monkey.operation |> worryManager

      let destinationMonkeyIdx =
        match worryLevel % monkey.divisibleTest = 0UL with
        | true -> monkey.trueCase
        | false -> monkey.falseCase

      let destinationMonkey =
        monkeys
        |> Map.find destinationMonkeyIdx
        |> (fun x -> { x with items = [ worryLevel ] |> List.append x.items })

      monkeys <-
        monkeys
        |> Map.add destinationMonkeyIdx destinationMonkey

    monkeys
    |> Map.add
         monkey.id
         { monkey with
             items = []
             inspectionCount = monkey.inspectionCount + inspectionCount }

  let handleRound monkeys =
    monkeys
    |> Map.keys
    |> Seq.sort
    |> Seq.fold (fun monkeys idx -> evolve monkeys (monkeys |> Map.find idx)) monkeys

  member _.Play() =
    let mutable monkeys = monkeys

    [ 1 .. n + 1 ]
    |> Seq.fold
         (fun rounds round ->
           let lastRound =
             rounds |> Map.find (round - 1)

           let currentRound = handleRound lastRound
           Map.add round currentRound rounds)
         (Map.ofList [ (0, monkeys) ])
    |> Map.find n
    |> Map.values
    |> Seq.map (fun m -> m.inspectionCount)
    |> Seq.sortDescending
    |> Seq.take 2
    |> Seq.reduce (*)

let testMonkeys = read "./test.txt" |> parse
let monkeys = read "./data.txt" |> parse

let toMap monkeys =
  monkeys
  |> Array.fold (fun map monkey -> map |> Map.add monkey.id monkey) Map.empty

let PartOneTest =
  PlayRounds(20, (fun x -> x / 3UL), testMonkeys |> toMap)

let PartOneActual =
  PlayRounds(20, (fun x -> x / 3UL), monkeys |> toMap)

PartOneTest.Play() |> printfn "Part I (TEST): %i"

PartOneActual.Play()
|> printfn "Part I (REAL): %i"

let getCommonMultiple monkeys =
  monkeys
  |> Array.map (fun x -> x.divisibleTest)
  |> Array.reduce (*)

let getPartTwoWorryFn monkeys =
  let value = getCommonMultiple monkeys
  fun x -> x % value

let PartTwoTest =
  PlayRounds(10000, getPartTwoWorryFn testMonkeys, testMonkeys |> toMap)

let PartTwoActual =
  PlayRounds(10000, getPartTwoWorryFn monkeys, monkeys |> toMap)

PartTwoTest.Play() |> printfn "Part II (TEST): %i"

PartTwoActual.Play()
|> printfn "Part II (REAL): %i"
