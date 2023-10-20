let input = System.IO.File.ReadAllText "./src/2016/09/input.txt"

type Marker = int * int

let joinSeq (sep: string) seq =
  let rec loop i acc =
    match seq |> Seq.tryItem i with
    | None -> acc
    | Some x ->
      acc + sep + x.ToString() |> loop (i + 1) 
    
  loop  0 ""
  
let split (x: string) (s: string) = s.Split x

let marker (chars: char array): Marker =
  chars
  |> Array.skip 1
  |> Array.takeWhile(fun x -> x <> ')')
  |> Seq.ofArray
  |> joinSeq ""
  |> split "x"
  |> (fun xs ->
      match xs with
      | [|x;y|] ->
        int x, int y
      | _ -> failwithf "bad data"
    )

let decompress (s: string) =
  let chars = s.ToCharArray()
  let rec loop curr i =
    match chars |> Array.tryItem i with
    | None -> curr
    | Some '(' ->
      let x,y = chars |> Array 
    
    curr 
  
  loop "" 0 

let tests = [ ("ADVENT", "ADVENT"); ("A(1x5)BC", "ABBBBBC") ]

tests
|> List.iter (fun (input, expected) ->
  let result = input |> decompress

  if result <> expected then
    failwithf "Given %s, expected %s, got %s" input expected result
  else
    ())
