let max = 33100000

let multiply x y = x * y

let partOne =
  let findAllDivisors n =
    let max = float n |> sqrt |> int
    { 1..max
    }
    |> Seq.filter (fun x -> n % x = 0)
    |> Seq.collect (fun x -> [ x; n / x ])
    |> Seq.distinct
    |> Seq.toArray
  
  let totalGifts =
    findAllDivisors >> Array.sumBy (multiply 10)  
  
  1
  |> Seq.unfold (fun x -> Some(x + 1, x + 1))
  |> Seq.filter (fun x -> totalGifts x >= max)
  |> Seq.head  

partOne |> (printfn "Part I: %i")

let partTwo =
  let findAllDivisors n =
    let max = float n |> sqrt |> int
    { 1..max
    }
    |> Seq.filter (fun x -> n % x = 0)
    |> Seq.collect (fun x -> [ x; n / x ])
    |> Seq.filter(fun x -> n / x <= 50)
    |> Seq.toArray
    
  let totalGifts =
    findAllDivisors >> Array.sumBy (multiply 11)  
  
  1
  |> Seq.unfold (fun x -> Some(x + 1, x + 1))
  |> Seq.filter (fun x -> totalGifts x >= max)
  |> Seq.head  

partTwo |> (printfn "Part II: %i")