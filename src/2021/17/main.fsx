type Input = { xs: int * int; ys: int * int }

module Input =
  let testData =
    "target area: x=20..30, y=-10..-5"

  let realData =
    "target area: x=244..303, y=-91..-54"

  let private parseDimension (s: string) =
    match s.Substring(2).Split("..") with
    | [| a; b |] ->
      printf "%A %A" a b
      Some(int a, int b)
    | _ -> None

  let targetArea (input: string) =
    match input.Substring(13).Split ", " with
    | xs -> xs |> Array.choose parseDimension
    |> function
      | [| xRange; yRange |] -> xRange, yRange
      | _ -> failwithf "Invalid input"


type State =
  { velocity: int * int
    position: int * int }

module State =
  let private getNextPosition (x, y) (vx, vy) = x + vx, y + vy

  let private getNextXVelocity vx =
    match vx with
    | 0 -> 0
    | v when v > 0 -> v - 1
    | v -> v + 1

  let private getNextYVelocity vy = vy - 1

  let private getNextVelocity (vx, vy) =
    vx |> getNextXVelocity, vy |> getNextYVelocity

  let step state =
    let nextPosition =
      getNextPosition state.position state.velocity

    { state with
        position = nextPosition
        velocity = getNextVelocity state.velocity }


let partOne input = input |> Input.targetArea

Input.testData
|> partOne
|> printfn "Part I (Test): %A"
