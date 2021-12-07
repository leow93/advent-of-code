type position = (int, int)
let initialPosition = (0, 0)

type direction = Forwards(int) | Up(int) | Down(int)

let evolve = ((x, y), direction) =>
  switch direction {
  | Forwards(v) => (x + v, y)
  | Up(v) => (x, y - v)
  | Down(v) => (x, y + v)
  }

let multiply = ((x, y)) => x * y

let parseFileInstructions = filename => {
  open NodeJs

  let parseLine = line => {
    switch line->Js.String2.split(" ") {
    | [direction, amountStr] => {
        let amount = amountStr->Belt.Int.fromString
        switch (direction, amount) {
        | ("forward", Some(amount)) => Forwards(amount)
        | ("up", Some(amount)) => Up(amount)
        | ("down", Some(amount)) => Down(amount)
        | _ => Js.Exn.raiseError("Could not parse line")
        }
      }
    | data => {
        Js.Console.log(data)
        Js.Exn.raiseError("Invalid line")
      }
    }
  }

  ["src", "2021", "02", filename]
  ->Path.resolve
  ->Fs.readFileSync
  ->Buffer.toString
  ->Js.String2.split("\n")
  ->Belt.Array.map(parseLine)
}

let main = () => {
  let instructions = parseFileInstructions("data.txt")
  let finalPosition = Belt.Array.reduce(instructions, initialPosition, evolve)
  Js.Console.log2("Part I: ", multiply(finalPosition))
}

main()