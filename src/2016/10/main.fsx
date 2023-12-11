open System
open System.Collections.Generic
open System.Text.RegularExpressions

type Target =
  | Bot of int
  | Output of int

type Bot =
  { Number: int
    Microchips: int list
    LowTarget: Target
    HighTarget: Target }



let input =
  System.IO.File.ReadAllLines "./input.txt"

let (|Regex|_|) pattern input =
  let m = Regex.Match(input, pattern)

  if m.Success then
    Some(List.tail [ for g in m.Groups -> g.Value ])
  else
    None

let (|Int|) x = Int32.Parse x

let (|Target|) =
  function
  | "bot" -> Bot
  | _ -> Output

module Input =

  let private foldLine bots (line: string) =
    match line with
    | Regex "bot (\d*) gives low to (bot|output) (\d*) and high to (bot|output) (\d*)" [ Int botN; Target lowT; Int lowN; Target hiT; Int hiN ] ->
      bots
      @ [ { Number = botN
            Microchips = []
            LowTarget = lowT lowN
            HighTarget = hiT hiN } ]
    | Regex "value (\d*) goes to bot (\d*)" [ Int value; Int botN ] ->
      bots
      |> List.map (fun b ->
        if b.Number = botN then
          { b with Microchips = value :: b.Microchips }
        else
          b)
    | _ -> bots

  let parse input =
    input |> Array.sort |> Array.fold foldLine []


let partOne =
  input
  |> Input.parse
  |> List.map (fun b -> b.Number, b)
  |> Map.ofList


let giveValue bots number value =
  bots
  |> Map.change number (function
    | Some b ->
      { b with Microchips = value :: b.Microchips }
      |> Some
    | None _ -> None)

let events =
  let bots = Dictionary<int, Bot>()

  input
  |> Array.sort
  |> Array.iter (fun line ->
    match line with
    | Regex "bot (\d*) gives low to (bot|output) (\d*) and high to (bot|output) (\d*)" 
            [ Int botN; Target lowT; Int lowN; Target hiT; Int hiN ] ->
      let bot = 
        { 
          Number     = botN
          Microchips = []
          LowTarget  = lowT lowN
          HighTarget   = hiT hiN 
        }
      bots.Add(botN, bot)

    | Regex "value (\d*) goes to bot (\d*)" [ Int value; Int botN ] ->
      let bot = bots.[botN]
      bots.[botN] <- { bot with Microchips = value::bot.Microchips })

  let give value = function
    | Bot botN -> 
      let bot = bots.[botN] 
      bots.[botN] <- { bot with Microchips = value::bot.Microchips }
    | _ -> ()

  let botsWithTwo = 
    bots 
    |> Seq.map (fun kvp -> kvp.Value)
    |> Seq.filter (fun bot -> bot.Microchips.Length = 2)

  seq {
    while Seq.length botsWithTwo > 0 do
      for bot in (Array.ofSeq botsWithTwo) do
        let [ lowVal; hiVal ] = bot.Microchips |> List.sort
        bots.[bot.Number] <- { bot with Microchips = [] }
        
        give lowVal bot.LowTarget
        give hiVal bot.HighTarget

        yield bot.Number, (bot.LowTarget, lowVal), (bot.HighTarget, hiVal)
  }

let part1 = 
  events 
  |> Seq.pick (fun (botN, (_, lowVal), (_, hiVal)) ->
    if lowVal = 17 && hiVal = 61 then Some botN else None)

part1 |> printfn "part I: %i"

let part2 =
  let isOutput0To2 = function
    | Output 0 | Output 1 | Output 2 -> true
    | _ -> false  

  events
  |> Seq.collect (fun (botN, (lowTarget, lowVal), (hiTarget, hiVal)) ->
     seq {
       if isOutput0To2 lowTarget then yield lowVal
       if isOutput0To2 hiTarget then yield hiVal
     })
  |> Seq.take 3
  |> Seq.reduce (*)
  
part2 |> printfn "part II: %i"
