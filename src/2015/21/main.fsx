
type Player =
  { name: string
    hp: int
    damage: int
    armour: int }

type Equipment =
  { cost: int
    damage: int
    armour: int }

type Shop =
  { weapons: Equipment []
    armour: Equipment []
    rings: Equipment [] }

let me =
  { name = "me"
    hp = 100
    damage = 0
    armour = 0 }

(*
Puzzle input

Hit Points: 109
Damage: 8
Armor: 2
*)
let boss =
  { name = "boss"
    hp = 109
    damage = 8
    armour = 2 }

let equipment cost damage armour =
  { cost = cost
    damage = damage
    armour = armour }

let shop =
  { weapons =
      [| equipment 8 4 0
         equipment 10 5 0
         equipment 25 6 0
         equipment 40 7 0
         equipment 74 8 0 |]
    armour =
      [| equipment 13 0 1
         equipment 31 0 2
         equipment 53 0 3
         equipment 75 0 4
         equipment 102 0 5 |]
    rings =
      [| equipment 25 1 0
         equipment 50 2 0
         equipment 100 3 0
         equipment 20 0 1
         equipment 40 0 2
         equipment 80 0 3 |] }

let weaponChoices =
  shop.weapons |> Seq.map (fun w -> [| w |])

let armourChoices =
  seq {
    yield [||]
    for a in shop.armour -> [| a |]
  }

let ringChoices =
  seq {
    yield [||]
    for r in shop.rings -> [| r |]

    for i in 0 .. shop.rings.Length - 1 do
      for j in i + 1 .. shop.rings.Length - 1 do
        yield [| shop.rings.[i]; shop.rings.[j] |]
  }

let equipmentCombos =
  seq {
    for w in weaponChoices do
      for a in armourChoices do
        for r in ringChoices do
          yield Array.concat [| w; a; r |]
  }

let revArgs fn a b = fn b a

let simulate p1 p2 =
  let rec loop (attacker: Player) (defender: Player) =
    let damage =
      max 1 (attacker.damage - defender.armour)

    let newHP = defender.hp - damage

    if newHP <= 0 then
      attacker.name
    else
      loop { defender with hp = newHP } attacker

  loop p1 p2

let provideEquipments (player: Player) equipments =
  { player with
      damage = equipments |> Array.sumBy (fun x -> x.damage)
      armour = equipments |> Array.sumBy (fun x -> x.armour) }


let partOne =
  equipmentCombos
  |> Seq.filter (fun equipments ->
    equipments
    |> provideEquipments me
    |> (revArgs simulate) boss
    |> (=) "me")
  |> Seq.map (Array.sumBy (fun x -> x.cost))
  |> Seq.min

printfn "Part I: %i" partOne

let partTwo =
  equipmentCombos
  |> Seq.filter (fun equipments ->
    equipments
    |> provideEquipments me
    |> (revArgs simulate) boss
    |> (=) "boss")
  |> Seq.map (Array.sumBy (fun x -> x.cost))
  |> Seq.max

printfn "Part II: %i" partTwo
