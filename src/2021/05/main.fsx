let input =
  System.IO.File.ReadAllLines "./data.txt"

let split (sep: string) (s: string) = s.Split sep

let parseInput input =
  input
  |> Array.choose (fun line ->
    line
    |> split " -> "
    |> function
      | [| a; b |] ->
        match a |> split ",", b |> split "," with
        | [| a; b |], [| c; d |] -> Some((int a, int b), (int c, int d))
        | _ -> None
      | _ -> None)

let sizeOfGrid input =
  input
  |> Array.fold (fun (maxX, maxY) ((x1, y1), (x2, y2)) -> (List.max [ maxX; x1; x2 ], List.max [ maxY; y1; y2 ])) (0, 0)

let makeGrid x y =
  ResizeArray(List.init (x + 1) (fun _ -> ResizeArray(List.init (y + 1) (fun _ -> 0))))

let plus a b = a + b
let minus a b = a - b

let main handleDiagonals =
  let parsed = input |> parseInput
  let maxX, maxY = sizeOfGrid parsed

  let grid = makeGrid maxX maxY
  let mutable counter = 0

  for line in parsed do
    let (x1, y1), (x2, y2) = line
    let startX = min x1 x2
    let endX = max x1 x2
    let startY = min y1 y2
    let endY = max y1 y2

    if (x1 = x2 || y1 = y2) then
      for x in { startX..endX } do
        for y in { startY..endY } do
          grid[x][y] <- grid[x][y] + 1

          if grid[x][y] = 2 then
            counter <- counter + 1
    elif handleDiagonals then
      let length = endY - startY

      let xOp =
        match x1 > x2 with
        | true -> minus
        | false -> plus

      let yOp =
        match y1 > y2 with
        | true -> minus
        | false -> plus

      for i in { 0..length } do
        let x, y = (xOp x1 i, yOp y1 i)
        grid[x][y] <- grid[x][y] + 1

        if grid[x][y] = 2 then
          counter <- counter + 1
    else
      ()

  counter


printfn "Part I: %A" (main false)

printfn "Part II: %A" (main true)
