open System.Collections.Generic

let readLines = System.IO.File.ReadAllLines

let neighbours grid (x, y) =
  [ (x + 1, y)
    (x - 1, y)
    (x, y + 1)
    (x, y - 1) ]
  |> List.choose (fun (x, y) ->
    match grid |> Array.tryItem x with
    | Some row ->
      match row |> Array.tryItem y with
      | Some x -> Some x 
      | _ -> None
    | _ -> None)

let findLowPoints (grid: int[][]) =
  grid
  |> Array.mapi(fun i row -> row |> Array.mapi(fun j c -> (i, j , c)))
  |> Array.fold(fun list row ->
      row |> Array.fold(fun list (i, j, cell) ->
          let xs = neighbours grid (i, j)
          if cell < List.min xs then
            list @ [(i,j)]
          else
            list
        ) list
    ) []

let riskLevel x = x + 1

let partOne file =
  let grid =
    readLines file
    |> Array.map (fun s -> s.ToCharArray() |> Array.map (string >> int))
  grid
  |> findLowPoints
  |> List.map(fun (i,j) -> grid[i][j] |> riskLevel)
  |> List.sum


partOne "./test.txt"
|> printfn "Part I (test): %i"

partOne "./data.txt"
|> printfn "Part I: %i"

let tryCoord (x, y) grid =
  match grid |> Array.tryItem x with
  | Some row ->
    match row |> Array.tryItem y with
    | None -> None
    | x -> x
  | None -> None

let isNewBasinPoint grid visited (x, y) =
  match visited |> tryCoord (x, y) with
  | None -> false
  | Some visited ->
    match grid |> tryCoord (x, y) with
    | None -> false
    | Some height -> not visited && height < 9

let rec basinSize grid visited (x, y) =
  if not (isNewBasinPoint grid visited (x, y)) then
    0
  else
    visited[x][y] <- true
    1 
    + basinSize grid visited (x - 1, y)
    + basinSize grid visited (x + 1, y)
    + basinSize grid visited (x, y - 1)
    + basinSize grid visited (x, y + 1)


let findBasins grid =
  let m,n = grid |> Array.length, grid[0] |> Array.length
  let visited =
    [|for _ in 0 .. m -> [| for _ in 0 .. n -> false  |] |]
  
  let basins = List<int>()
  for i,j in findLowPoints grid do
    if isNewBasinPoint grid visited (i, j) then
      basins.Add(basinSize grid visited (i, j))
  
  basins
   

let partTwo file =
  let grid =
    readLines file
    |> Array.map (fun s -> s.ToCharArray() |> Array.map (string >> int))
    
  grid
  |> findBasins
  |> Seq.sortDescending
  |> Seq.take 3
  |> Seq.reduce (*)
  
      
partTwo "./test.txt"
|> printfn "Part II (test): %i"

partTwo "./data.txt"
|> printfn "Part II: %i"