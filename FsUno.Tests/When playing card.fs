module FsUno.Tests.``When playing card``

open FsUno.Domain
open Deck
open Game

open Specifications
open System
open Xunit

let gameId = GameId 1

[<Fact>]
let ``Same color should be accepted``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3; FirstPlayer = 0 } ]
    |> When ( PlayCard { GameId = gameId; Player = 0; Card = red 9 } )
    |> Expect [ CardPlayed { GameId = gameId; Player = 0; Card = red 9; NextPlayer = 1 } ]

[<Fact>]
let ``Same value should be accepted``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3; FirstPlayer = 0 } ]
    |> When ( PlayCard { GameId = gameId; Player = 0; Card = yellow 3 } )
    |> Expect [ CardPlayed { GameId = gameId; Player = 0; Card = yellow 3; NextPlayer = 1 } ]

[<Fact>]
let ``Different value and color should be rejected``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3; FirstPlayer = 0 } ]
    |> When ( PlayCard { GameId = gameId; Player = 0; Card = yellow 8 } )
    |> Expect [ PlayerPlayedWrongCard { GameId = gameId; Player = 0; Card = yellow 8}]

[<Fact>]
let ``First player should play at his turn``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 3; FirstPlayer = 0 } ]
    |> When ( PlayCard { GameId = gameId; Player = 2; Card = green 3 } )
    |> Expect [ PlayerPlayedAtWrongTurn { GameId = gameId; Player = 2; Card = green 3 }]

[<Fact>]
let ``First player player after starting with kickback should be the dealer, next one should be on the right``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = KickBack Green; FirstPlayer = 0 } 
            DirectionChanged { GameId = gameId; Direction = CounterClockWise }]
    |> When ( PlayCard { GameId = gameId; Player = 0; Card = green 3 } )
    |> Expect [ CardPlayed { GameId = gameId; Player = 0; Card = green 3; NextPlayer = 3 }]

[<Fact>]
let ``First player player after starting with skip should be the one after the dealer``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = Skip Green; FirstPlayer = 1 } ]
    |> When ( PlayCard { GameId = gameId; Player = 1; Card = green 3 } )
    |> Expect [ CardPlayed { GameId = gameId; Player = 1; Card = green 3; NextPlayer = 2 }]
