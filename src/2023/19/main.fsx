#load "../../utils/Utils.fsx"

open System.Collections.Generic
open Utils

let input = Input.readText ()

type Key =
  | X
  | M
  | A
  | S

type Part = Map<Key, int64>

type Operator =
  | Gt
  | Lt

type Comparison =
  { key: Key
    operator: Operator
    value: int64
    destination: Destination }

and Destination =
  | Workflow of string
  | Terminal of TerminalState

and TerminalState =
  | Accepted
  | Rejected

type Rule =
  | GoTo of Destination
  | Comparison of Comparison

module PartsParser =
  let private parseLine (line: string) =
    let line = line.Replace("{", "").Replace("}", "")

    match line |> Strings.split "," with
    | [| x; m; a; s |] ->
      let parseInt (x: string) = x.Substring(2) |> int64

      [ (X, parseInt x); (M, parseInt m); (A, parseInt a); (S, parseInt s) ]
      |> Map.ofList
      |> Some
    | _ -> None

  let parse = Array.choose parseLine

module RuleParser =
  let private parseDestination (dest: string) =
    match dest with
    | "R" -> Terminal Rejected
    | "A" -> Terminal Accepted
    | x -> (Workflow x)

  let parseRule (rule: string) =
    if not (rule.Contains(":")) then
      match rule with
      | "A" -> Terminal Accepted
      | "R" -> Terminal Rejected
      | x -> Workflow x
      |> GoTo
      |> Some
    else
      match rule.Split([| ':' |]) with
      | [| cmp; dest |] ->
        let destination = parseDestination dest

        match cmp.Split([| '<'; '>' |]) with
        | [| category; amount |] ->
          let amount = int amount
          let operator = if cmp.Contains "<" then Lt else Gt

          let rule =
            { key =
                match category with
                | "x" -> X
                | "m" -> M
                | "a" -> A
                | "s" -> S
                | _ -> failwith "Unexpected category"
              operator = operator
              value = amount
              destination = destination }

          Some(Comparison rule)
        | _ -> None
      | _ -> None

module WorkflowsParser =
  let private parseRule (spec: string) = Some spec

  let private workflowParser (line: string) =
    let line = line.Replace("}", "")

    match line |> Strings.split "{" with
    | [| id; spec |] ->
      let workflowRules = spec |> Strings.split "," |> Array.choose RuleParser.parseRule
      Some(id, workflowRules)

    | _ -> None

  let parse = Array.choose workflowParser

module Parser =
  let parse (text: string) =
    let splitNewLine = Strings.split "\n"

    match text |> Strings.split "\n\n" with
    | [| workflowRules; parts |] -> workflowRules |> splitNewLine |> WorkflowsParser.parse |> Map.ofArray, parts |> splitNewLine |> PartsParser.parse
    | _ -> failwith "Unexpected input"

let getPartDestination (workflows: Map<string, Rule[]>) (part: Part) : TerminalState =
  let rec inner workflowId =
    let rules = workflows[workflowId]
    let mutable destination = None
    let mutable i = 0

    while (destination.IsNone && i < rules.Length) do
      let rule = rules[i]

      match rule with
      | GoTo x -> destination <- Some x
      | Comparison rule ->
        match rule.key, rule.operator with
        | X, Gt ->
          if part[X] > rule.value then
            destination <- Some rule.destination
        | X, Lt ->
          if part[X] < rule.value then
            destination <- Some rule.destination
        | M, Gt ->
          if part[M] > rule.value then
            destination <- Some rule.destination
        | M, Lt ->
          if part[M] < rule.value then
            destination <- Some rule.destination
        | A, Gt ->
          if part[A] > rule.value then
            destination <- Some rule.destination
        | A, Lt ->
          if part[A] < rule.value then
            destination <- Some rule.destination
        | S, Gt ->
          if part[S] > rule.value then
            destination <- Some rule.destination
        | S, Lt ->
          if part[S] < rule.value then
            destination <- Some rule.destination

      i <- i + 1

    match destination with
    | None -> failwith "No destination found"
    | Some(Terminal Accepted) -> Accepted
    | Some(Terminal Rejected) -> Rejected
    | Some(Workflow x) -> inner x

  inner "in"

let loopWorkflows (workflows: Map<string, Rule[]>) parts =
  let rec inner i acceptedParts =
    match parts |> Array.tryItem i with
    | None -> acceptedParts
    | Some part ->
      match getPartDestination workflows part with
      | Accepted -> inner (i + 1) (acceptedParts @ [ part ])
      | Rejected -> inner (i + 1) acceptedParts

  inner 0 []

let partOne input =
  let workflows, data = input |> Parser.parse
  let acceptedParts = loopWorkflows workflows data
  acceptedParts |> List.sumBy (fun x -> x[X] + x[M] + x[A] + x[S])

type Range = int64 * int64

type RangeState = { ranges: Dictionary<Key, Range> }

let getMin (key: Key) (state: RangeState) = state.ranges[key] |> fst
let getMax (key: Key) (state: RangeState) = state.ranges[key] |> snd

let partTwo input =
  let workflows, _ = input |> Parser.parse

  let getName =
    function
    | Workflow x -> x
    | Terminal Accepted -> "A"
    | Terminal Rejected -> "R"

  let rec countRanges (ranges: Dictionary<Key, Range>) id (workflows: Map<string, Rule[]>) =

    if id = "R" then
      0L
    elif id = "A" then
      ranges.Values |> Seq.fold (fun acc (lo, hi) -> acc * (1L + hi - lo)) 1L
    else
      let mutable total = 0L
      let rules = workflows[id]

      for rule in rules do
        match rule with
        | GoTo x -> total <- total + countRanges ranges (getName x) workflows
        | Comparison r ->
          let lo, hi = ranges[r.key]

          let trueForComparison =
            if r.operator = Lt then
              (lo, r.value - 1L)
            else
              (r.value + 1L, hi)

          let falseForComparison = if r.operator = Lt then (r.value, hi) else (lo, r.value)

          if fst trueForComparison <= snd trueForComparison then
            let newRanges = Dictionary ranges
            newRanges[r.key] <- trueForComparison
            total <- total + countRanges newRanges (getName r.destination) workflows

          if fst falseForComparison <= snd falseForComparison then
            ranges[r.key] <- falseForComparison


      total

  let initialRange = 1L, 4000L

  let ranges =
    [ (X, initialRange); (M, initialRange); (A, initialRange); (S, initialRange) ]
    |> Map.ofList
    |> Dictionary

  countRanges ranges "in" workflows

partOne input |> printfn "Part one: %A"
partTwo input |> printfn "Part two: %A"
