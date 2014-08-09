module FsUno.Tests.``When playing after a player who failed``

open FsUno.Domain
open Deck
open Game

open Specifications
open Xunit

let gameId = GameId 1

[<Fact>]
let ``After wrong color it should still be player's turn``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3 }
            CardPlayed { GameId = gameId; Player = 0; Card = red 9; NextPlayer = 1}
            PlayerPlayedWrongCard { GameId = gameId; Player = 1; Card = green 7 } ]
    |> When (PlayCard { GameId = gameId; Player = 1; Card = green 9 })
    |> Expect [ CardPlayed { GameId = gameId; Player = 1; Card = green 9; NextPlayer = 2}]

    

[<Fact>]
let ``After wrong turn it should still be expected player's turn``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3 }
            CardPlayed { GameId = gameId; Player = 0; Card = red 9; NextPlayer = 1}
            PlayerPlayedAtWrongTurn { GameId = gameId; Player = 3; Card = green 7 } ]
    |> When (PlayCard { GameId = gameId; Player = 1; Card = green 9 })
    |> Expect [ CardPlayed { GameId = gameId; Player = 1; Card = green 9; NextPlayer = 2}]

    

