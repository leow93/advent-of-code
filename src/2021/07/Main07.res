open Belt

let readLines = filename => {
  open NodeJs
  ["src", "2021", "07", filename]->Path.resolve->Fs.readFileSync->Buffer.toString
}

let parseInput = s => s->Js.String2.split(",")->Array.keepMap(Int.fromString)
let fst = list => list->Js.Array2.shift

let minimum = (list: array<int>) => {
  switch list->SortArray.Int.stableSort->fst {
  | Some(x) => x
  | None => 0
  }
}

let maximum = (list: array<int>) => {
  switch list->SortArray.Int.stableSort->Js.Array2.pop {
  | Some(x) => x
  | None => 0
  }
}

let getFuelCosts = (positions, fuelCostFn) => {
  let min = positions->minimum
  let max = positions->maximum

  Array.range(min, max)
  ->Array.reduce(Map.Int.empty, (map, position) => {
    let distances = Array.make(positions->Array.length, 0)
    for i in 0 to positions->Array.length {
      distances->Array.setUnsafe(i, fuelCostFn(positions->Array.getUnsafe(i), position))
    }
    map->Map.Int.set(position, distances->Array.reduce(0, (sum, distance) => sum + distance))
  })
  ->Map.Int.toArray
  ->SortArray.stableSortBy((a, b) => {
    let (_, fuelCostA) = a
    let (_, fuelCostB) = b

    fuelCostA > fuelCostB ? 1 : -1
  })
  ->fst
}

let diff = (a, b) => (a - b)->Js.Math.abs_int

let partOne = input => {
  Js.log2("Part I:", input->parseInput->getFuelCosts(diff))
}

let sumFromOneToN = n => n * (n + 1) / 2

let partTwo = input => {
  Js.log2("Part II:", input->parseInput->getFuelCosts((x, y) => diff(x, y)->sumFromOneToN))
}

let main = () => {
  let testLines = readLines("test.txt")
  let data = readLines("data.txt")

  Js.log("Test")
  testLines->partOne
  testLines->partTwo
  Js.log("\n")

  Js.log("Actual")
  data->partOne
  data->partTwo
}

main()