type Player =
  { hp: int
    armour: int
    mana: int }

type Boss = { hp: int; damage: int }

type Effect = Player * Boss -> Player * Boss

type Game =
  { player: Player
    boss: Boss
    effects: (string * Effect * int) list }

type Result =
  | PlayerWin
  | BossWin

type Spell = Game -> Game

[<AutoOpen>]
module Spells =

  let private hitBoss damage (boss: Boss) =
    { boss with hp = boss.hp - damage }

  let private costMana cost player =
    { player with mana = player.mana - cost }

  let private heal hp (player: Player) =
    { player with hp = player.hp + hp }

  let missile game =
    { game with
        boss = hitBoss 4 game.boss
        player = costMana 53 game.player }

  let drain game =
    { game with
        boss = hitBoss 2 game.boss
        player = costMana 73 game.player |> heal 2 }

  let private shieldEffect (player: Player, boss: Boss) =
    { player with armour = player.armour + 7 }, boss

  let shield game =
    { game with
        player = costMana 113 game.player
        effects = ("shield", shieldEffect, 6) :: game.effects }

  let private poisonEffect (player: Player, boss: Boss) = player, hitBoss 3 boss

  let poison game =
    { game with
        player = costMana 173 game.player
        effects = ("poison", poisonEffect, 6) :: game.effects }

  let private rechargeEffect (player: Player, boss: Boss) =
    { player with mana = player.mana + 101 }, boss

  let recharge game =
    { game with
        player = costMana 229 game.player
        effects = ("recharge", rechargeEffect, 5) :: game.effects }

  let spells =
    [| "missile", missile, 53
       "drain", drain, 73
       "shield", shield, 113
       "poison", poison, 173
       "recharge", recharge, 229 |]

let you =
  { hp = 50
    armour = 0
    mana = 500 }

(*
Puzzle Input

Hit Points: 71
Damage: 10
*)
let boss =
  { hp = 71; damage = 10 }

let hitPlayer damage (player: Player) =
  let damage =
    max 1 (damage - player.armour)

  { player with hp = player.hp - damage }

let applyEffects game =
  // first reset player's armour to 0 before applying the
  // shield effects below
  let player =
    { game.player with armour = 0 }

  let boss = game.boss

  // recursively apply the effects
  let player, boss =
    game.effects
    |> List.fold (fun x (_, effect, _) -> effect x) (player, boss)

  let effects =
    game.effects
    |> List.map (fun (spell, effect, n) -> spell, effect, n - 1)
    |> List.filter (fun (_, _, n) -> n > 0)

  { game with
      player = player
      boss = boss
      effects = effects }

let (|IsGameOver|_|) { player = player; boss = boss } =
  if player.hp <= 0 then
    Some BossWin
  elif boss.hp <= 0 then
    Some PlayerWin
  else
    None

let runSim player boss fn =
  let rec playerTurn game totalMana =
    let game = fn game
    match applyEffects game with
    | IsGameOver result -> seq { yield result, totalMana }

    // not enough mana to cast any spell = instant lose
    | game when game.player.mana < 53 -> seq { yield BossWin, totalMana }

    | game ->
      spells
      |> Seq.filter (fun (_, _, cost) -> cost <= game.player.mana)
      |> Seq.filter (fun (spell, _, _) ->
        game.effects
        |> List.forall (fun (spell', _, _) -> spell <> spell'))
      |> Seq.collect (fun (_, castSpell, cost) ->
        let game = castSpell game
        let totalMana = totalMana + cost

        if game.boss.hp <= 0 then
          seq { yield PlayerWin, totalMana }
        else
          bossTurn game totalMana)

  and bossTurn game acc =
    match applyEffects game with
    | IsGameOver result -> seq { yield result, acc }

    | { player = player; boss = boss } as game ->
      let player = hitPlayer boss.damage player

      if player.hp <= 0 then
        seq { yield BossWin, acc }
      else
        playerTurn { game with player = player } acc

  let game =
    { player = player
      boss = boss
      effects = [] }

  playerTurn game 0

runSim you boss id
|> Seq.filter (fst >> (=) PlayerWin)
|> Seq.map snd
|> Seq.min
|> printfn "Part I: %i"

runSim you boss (fun game -> {game with player = hitPlayer 1 game.player })
|> Seq.filter (fst >> (=) PlayerWin)
|> Seq.map snd
|> Seq.min
|> printfn "Part II: %i"
