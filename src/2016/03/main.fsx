open System

let validTriangle (a, b, c) = a + b > c && b + c > a && a + c > b
let toArray (a, b, c) = [| a; b; c |]

let toTriple =
  function
  | [| a; b; c |] -> (a, b, c) |> Some
  | _ -> None

let input =
  System.IO.File.ReadAllLines "./input.txt"

let triangles =
  input
  |> Array.choose (fun line ->
    line.Split()
    |> Array.filter (not << String.IsNullOrWhiteSpace)
    |> Array.map int
    |> toTriple)

let countValidTriangles triangles =
  triangles
  |> Seq.filter validTriangle
  |> Seq.length

let partOne =
  triangles |> countValidTriangles

printfn "Part I: %i" partOne

let partTwoTriangles =
  triangles
  |> Array.map toArray
  |> Array.transpose
  |> Array.map (Array.chunkBySize 3 >> Array.choose toTriple)
  |> Array.concat
let partTwo =
  partTwoTriangles |> countValidTriangles

printfn "Part II: %A" partTwo
