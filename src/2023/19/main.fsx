#load "../../utils/Utils.fsx"

open Utils
let input = Input.readText ()
type Part = { a: int; x: int; m: int; s: int }

module PartsParser =
  let private parseLine (line: string) =
    let line = line.Replace("{", "").Replace("}", "")

    match line |> Strings.split "," with
    | [| x; m; a; s |] ->
      Some
        { x = x.Substring(2) |> int
          m = m.Substring(2) |> int
          a = a.Substring(2) |> int
          s = s.Substring(2) |> int }
    | _ -> None

  let parse = Array.choose parseLine

module RuleParser =
  type TerminalState =
    | Accepted
    | Rejected
  type Destination =
    | Workflow of string
    | Terminal of TerminalState
    
  type Condition =
    | Comparison of (Part -> Destination option)
    | Direct of Destination

  let private parseDestination (dest: string) =
    match dest with
    | "R" -> Terminal Rejected
    | "A" -> Terminal Accepted
    | x -> (Workflow x)

  let parseRule (rule: string) : Condition option =
    if not (rule.Contains(":")) then
      let destination = parseDestination rule
      Direct destination |> Some
    else
      match rule.Split([| ':' |]) with
      | [| cmp; dest |] ->
        let destination = parseDestination dest

        match cmp.Split([| '<'; '>' |]) with
        | [| category; amount |] ->
          let amount = int amount
          let operator = if cmp.Contains "<" then (<) else (>)

          let comparison =
            fun (part: Part) ->
              match category with
              | "x" -> if (operator part.x amount) then Some destination else None
              | "m" -> if (operator part.m amount) then Some destination else None
              | "a" -> if (operator part.a amount) then Some destination else None
              | "s" -> if (operator part.s amount) then Some destination else None
              | _ -> None

          Some(Comparison comparison)
        | _ -> None
      | _ -> None

open RuleParser

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

open RuleParser

let getPartDestination (workflows: Map<string, Condition[]>) part : TerminalState =
  let rec inner workflowId =
    let rules = workflows[workflowId]
    let mutable destination = None
    let mutable i = 0;
    while (destination.IsNone && i < rules.Length) do
      let rule = rules[i]
      match rule with
      | Direct x -> destination <- Some x
      | Comparison cmp -> destination <- cmp part
      i <- i + 1
   
    match destination with
    | None -> failwith "No destination found"
    | Some (Workflow x) -> inner x
    | Some (Terminal Accepted) -> Accepted
    | Some (Terminal Rejected) -> Rejected
  
  inner "in"

let loopWorkflows (workflows: Map<string, Condition[]>) parts =
  let rec inner i acceptedParts =
    match parts |> Array.tryItem i with
    | None -> acceptedParts
    | Some part ->
      match getPartDestination workflows part with
      | Accepted -> inner (i + 1) (acceptedParts @ [part])
      | Rejected -> inner (i + 1) acceptedParts
  
  inner 0 []  

let partOne input =
  let workflows, data = input |> Parser.parse
  let acceptedParts = loopWorkflows workflows data
  acceptedParts |> List.sumBy (fun x -> x.a + x.x + x.m + x.s)

partOne input |> printfn "%A"
