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
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red Three; FirstPlayer = 0 } ]
    |> When ( PlayCard { GameId = gameId; Player = 0; Card = red Nine } )
    |> Expect [ CardPlayed { GameId = gameId; Player = 0; Card = red Nine; NextPlayer = 1 } ]

[<Fact>]
let ``Same value should be accepted``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red Three; FirstPlayer = 0 } ]
    |> When ( PlayCard { GameId = gameId; Player = 0; Card = yellow Three } )
    |> Expect [ CardPlayed { GameId = gameId; Player = 0; Card = yellow Three; NextPlayer = 1 } ]

[<Fact>]
let ``Different value and color should be rejected``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red Three; FirstPlayer = 0 } ]
    |> When ( PlayCard { GameId = gameId; Player = 0; Card = yellow Eight } )
    |> Expect [ PlayerPlayedWrongCard { GameId = gameId; Player = 0; Card = yellow Eight}]

[<Fact>]
let ``First player should play at his turn``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red Three; FirstPlayer = 0 } ]
    |> When ( PlayCard { GameId = gameId; Player = 2; Card = green Three } )
    |> Expect [ PlayerPlayedAtWrongTurn { GameId = gameId; Player = 2; Card = green Three }]

[<Fact>]
let ``First player player after starting with kickback should be the dealer, next one should be on the right``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = KickBack Green; FirstPlayer = 0 } 
            DirectionChanged { GameId = gameId; Direction = CounterClockWise }]
    |> When ( PlayCard { GameId = gameId; Player = 0; Card = green Three } )
    |> Expect [ CardPlayed { GameId = gameId; Player = 0; Card = green Three; NextPlayer = 3 }]

[<Fact>]
let ``First player player after starting with skip should be the one after the dealer``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = Skip Green; FirstPlayer = 1 } ]
    |> When ( PlayCard { GameId = gameId; Player = 1; Card = green Three } )
    |> Expect [ CardPlayed { GameId = gameId; Player = 1; Card = green Three; NextPlayer = 2 }]
