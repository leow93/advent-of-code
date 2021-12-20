open Belt

let empty = Map.Int.fromArray([
  (0, 0.),
  (1, 0.),
  (2, 0.),
  (3, 0.),
  (4, 0.),
  (5, 0.),
  (6, 0.),
  (7, 0.),
  (8, 0.),
])

let simulateDay = map => {
  map
  ->Map.Int.set(0, map->Map.Int.getWithDefault(1, 0.))
  ->Map.Int.set(1, map->Map.Int.getWithDefault(2, 0.))
  ->Map.Int.set(2, map->Map.Int.getWithDefault(3, 0.))
  ->Map.Int.set(3, map->Map.Int.getWithDefault(4, 0.))
  ->Map.Int.set(4, map->Map.Int.getWithDefault(5, 0.))
  ->Map.Int.set(5, map->Map.Int.getWithDefault(6, 0.))
  ->Map.Int.set(6, map->Map.Int.getWithDefault(7, 0.) +. map->Map.Int.getWithDefault(0, 0.))
  ->Map.Int.set(7, map->Map.Int.getWithDefault(8, 0.))
  ->Map.Int.set(8, map->Map.Int.getWithDefault(0, 0.))
}

let rec simulateDays = (map: Map.Int.t<float>, count) => {
  switch count {
  | 0 => map
  | n => simulateDays(map->simulateDay, n - 1)
  }
}

let incrementKey = (map, k) => {
  switch k {
  | Some(x) => map->Map.Int.set(x, map->Map.Int.getWithDefault(x, 0.) +. 1.)
  | None => map
  }
}

let parseInput = s =>
  s->Js.String2.split(",")->Array.map(Int.fromString)->Array.reduce(empty, incrementKey)

let partOne = input => {
  Js.log2(
    "Part I:",
    parseInput(input)->simulateDays(80)->Map.Int.reduce(0., (count, _, v) => count +. v),
  )
}

let partTwo = input => {
  Js.log2(
    "Part II:",
    parseInput(input)->simulateDays(256)->Map.Int.reduce(0., (count, _, v) => count +. v),
  )
}

let readLines = filename => {
  open NodeJs
  ["06", filename]->Path.resolve->Fs.readFileSync->Buffer.toString
}

let testLines = readLines("test.txt")
let data = readLines("data.txt")

Js.log("Test")
testLines->partOne
testLines->partTwo
Js.log("\n")

Js.log("Actual")
data->partOne
data->partTwo