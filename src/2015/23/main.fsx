open System.Collections.Generic

type Instruction =
  | Half of string
  | Triple of string
  | Increment of string
  | Jump of int
  | JumpIfEven of string * int
  | JumpIfOne of string * int

module Input =
  let private split (sep: string) (s: string) = s.Split sep |> List.ofArray

  let private parseLine (line: string) =
    match split ", " line with
    | [ a; b ] ->
      match split " " a with
      | [ "jio"; x ] -> JumpIfOne(x, int b) |> Some
      | [ "jie"; x ] -> JumpIfEven(x, int b) |> Some
      | _ -> None
    | _ ->
      match split " " line with
      | [ "hlf"; x ] -> Half x |> Some
      | [ "tpl"; x ] -> Triple x |> Some
      | [ "inc"; x ] -> Increment x |> Some
      | [ "jmp"; x ] -> Jump(int x) |> Some
      | _ -> None

  let parseFile file =
    System.IO.File.ReadAllLines file
    |> Array.choose parseLine

module State =
  let runInstructions initialState instructions =
    let rec loop (state: Map<string, uint64>) idx =
      match instructions |> Array.tryItem idx with
      | None -> state
      | Some (Half x) -> loop (state.Add(x, state[x] / 2UL)) (idx + 1)
      | Some (Triple x) ->
        let value = state[x] * 3UL
        loop (state.Add(x, value)) (idx + 1)
      | Some (Increment x) ->
        let value = state[x] + 1UL
        loop (state.Add(x, value)) (idx + 1)
      | Some (Jump x) -> loop state (idx + x)
      | Some (JumpIfEven (x, y)) ->
        if state[x] % 2UL = 0UL then
          loop state (idx + y)
        else
          loop state (idx + 1)
      | Some (JumpIfOne (x, y)) ->
        if state[x] = 1UL then
          loop state (idx + y)
        else
          loop state (idx + 1)

    loop initialState 0


let partOne file key =
  Input.parseFile file
  |> State.runInstructions (Map.ofArray [| ("a", 0UL); ("b", 0UL) |])
  |> (fun x -> x[key])
  |> printfn "Part I (%s): %i" file

partOne "./test.txt" "a"
partOne "./data.txt" "b"

let partTwo file key =
  Input.parseFile file
  |> State.runInstructions (Map.ofArray [| ("a", 1UL); ("b", 0UL) |])
  |> (fun x -> x[key])
  |> printfn "Part II (%s): %i" file

partTwo "./test.txt" "a"
partTwo "./data.txt" "b"