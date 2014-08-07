module FsUno.Tests.``When playing card``

open Xunit
open System
open Specifications
open Deck
open Game

let gameId = GameId 1
[<Fact>]
let ``Same color should be accepted``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3 } ]
    |> When ( PlayCard { GameId = gameId; Player = 0; Card = red 9 } )
    |> Expect [ CardPlayed { GameId = gameId; Player = 0; Card = red 9; NextPlayer = 1 } ]

[<Fact>]
let ``Same value should be accepted``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3 } ]
    |> When ( PlayCard { GameId = gameId; Player = 0; Card = yellow 3 } )
    |> Expect [ CardPlayed { GameId = gameId; Player = 0; Card = yellow 3; NextPlayer = 1 } ]

[<Fact>]
let ``Different value and color should be rejected``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3 } ]
    |> When ( PlayCard { GameId = gameId; Player = 0; Card = yellow 8 } )
    |> ExpectThrows<InvalidOperationException>

[<Fact>]
let ``First player should play at his turn``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3 } ]
    |> When ( PlayCard { GameId = gameId; Player = 2; Card = green 3 } )
    |> Expect [ PlayerPlayedAtWrongTurn { GameId = gameId; Player = 2; Card = green 3 }]
