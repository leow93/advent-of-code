open System.Collections.Generic

let pairs =
  Map(
    [ ('(', ')')
      ('[', ']')
      ('{', '}')
      ('<', '>') ]
  )

let isMatch (a, b) = pairs |> Map.find a = b

let findCorruption (s: string) =
  let stack = Stack<char>()
  let chars = s.ToCharArray()

  let rec loop idx =
    match chars |> Array.tryItem idx with
    | None -> None
    | Some char ->
      match char with
      | '['
      | '<'
      | '{'
      | '(' as x ->
        stack.Push x
        loop (idx + 1)
      | ']'
      | '>'
      | '}'
      | ')' as x ->
        if not (isMatch (stack.Pop(), x)) then
          Some x
        else
          loop (idx + 1)
      | x -> failwithf "unrecognised bracket %c" x

  loop 0

let scorePartOne =
  function
  | ')' -> 3
  | ']' -> 57
  | '}' -> 1197
  | '>' -> 25137
  | x -> failwithf "unrecognised bracket %c" x

let input =
  System.IO.File.ReadAllLines "./data.txt"

let partOne =
  input
  |> Seq.choose findCorruption
  |> Seq.map scorePartOne
  |> Seq.sum

partOne |> printfn "Part I: %i"

let findClosingSequence (s: string) =
  let chars = s.ToCharArray()
  let stack = Stack<char>()

  for char in chars do
    match char with
    | '['
    | '<'
    | '{'
    | '(' as x -> stack.Push x
    | ']'
    | '>'
    | '}'
    | ')' -> stack.Pop() |> ignore
    | x -> failwithf "unrecognised bracket %c" x

  [ for x in stack -> pairs |> Map.find x ]

let scorePartTwo =
  function
  | ')' -> 1UL
  | ']' -> 2UL
  | '}' -> 3UL
  | '>' -> 4UL
  | x -> failwithf "unrecognised bracket %c" x

let computeScore seq =
  seq
  |> Seq.fold (fun score bracket -> score * 5UL + scorePartTwo bracket) 0UL

let partTwo =
  input
  |> Seq.filter (findCorruption >> Option.isNone)
  |> Seq.map (findClosingSequence >> computeScore)
  |> Seq.sort
  |> Seq.toArray
  |> (fun xs -> xs[xs.Length / 2])

partTwo |> printfn "Part II: %i"

#r "nuget: Xunit"
open Xunit

Assert.Equal(None, findCorruption "()")
Assert.Equal(None, findCorruption "([<>])")
Assert.Equal(Some ']', findCorruption "(]")
Assert.Equal(Some '>', findCorruption "{()()()>")
