open System.Text.RegularExpressions

type Reindeer =
  { name: string
    speed: int
    restsAfter: int
    restsFor: int
    clock: int
    distance: int }

module Input =
  let private regex =
    "(\w+) can fly (\d+) km/s for (\d+) seconds, but then must rest for (\d+) seconds\."

  let private parseLine (line: string) : Reindeer =
    Regex.Match(line, regex).Groups
    |> (fun groups ->
      { name = groups[1].Value
        speed = (groups[2].Value |> int)
        restsAfter = (groups[3].Value |> int)
        restsFor = (groups[4].Value |> int)
        clock = 0
        distance = 0 })


  let readFile = System.IO.File.ReadAllLines
  let parseLines (lines: string []) = lines |> Array.map parseLine

module ReindeerProgress =
  let nextState (state: Reindeer) : Reindeer =
    let normalizedTime =
      state.clock % (state.restsFor + state.restsAfter)

    let isResting =
      normalizedTime >= state.restsAfter

    if isResting then
      { state with clock = state.clock + 1 }
    else
      { state with
          clock = state.clock + 1
          distance = state.distance + state.speed }

  let rec moveUntil x (state: Reindeer) =
    if state.clock = x then
      state
    else
      moveUntil x (nextState state)

let partOne file =
  let reindeer =
    Input.readFile file |> Input.parseLines

  reindeer
  |> Array.map (ReindeerProgress.moveUntil 1000)
  |> Array.sortByDescending (fun x -> x.distance)
  |> Array.head
  |> (fun x -> x.distance)
  |> printfn "Part I: %i"

let partTwo file =
  let reindeer =
    Input.readFile file |> Input.parseLines

  let max = 2503

  let rec loop reindeer points n =
    if n = max then
      points
    else
      let reindeer' =
        reindeer |> Array.map ReindeerProgress.nextState

      match reindeer'
            |> Array.sortByDescending (fun x -> x.distance)
            |> Array.tryHead
        with
      | Some winner ->
        let points' =
          points
          |> Map.change winner.name (function
            | None -> Some 1
            | Some x -> Some(x + 1))

        loop reindeer' points' (n + 1)
      | None -> loop reindeer' points (n + 1)

  loop reindeer Map.empty 0
  |> Map.values
  |> Seq.max
  |> printfn "Part II: %i"

partOne "./data.txt"
partTwo "./data.txt"
