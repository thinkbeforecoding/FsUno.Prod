module FsUno.Tests.``When starting game``

open FsUno.Domain
open Deck
open Game

open Specifications
open System
open Xunit

let gameId = GameId 1

[<Fact>]
let ``Started game should be started``() =
    Given []
    |> When ( StartGame { GameId = gameId; PlayerCount = 4; FirstCard = red 3 } )
    |> Expect [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3 } ]

[<Fact>]
let ``0 players should be rejected``() =
    Given []
    |> When ( StartGame { GameId = gameId; PlayerCount = 0; FirstCard = red 3 } )
    |> ExpectThrows<ArgumentException>


[<Fact>]
let ``Game should not be started twice``() =
    Given [GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3 } ]
    |> When ( StartGame { GameId = gameId; PlayerCount = 4; FirstCard = red 2 } )
    |> ExpectThrows<InvalidOperationException>

[<Fact>]
let ``Starting with a kickback should change direction``() =
    Given []
    |> When ( StartGame { GameId = gameId; PlayerCount = 4; FirstCard = KickBack Yellow })
    |> Expect [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = KickBack Yellow}
                DirectionChanged { GameId = gameId; Direction = CounterClockWise } ]