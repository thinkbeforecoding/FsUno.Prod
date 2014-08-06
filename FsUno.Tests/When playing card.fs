module FsUno.Tests.``When playing card``

open Xunit
open System
open Specifications
open Deck
open Game

[<Fact>]
let ``Same color should be accepted``() =
    Given [ GameStarted { GameId = 1; PlayerCount = 4; FirstCard = red 3 } ]
    |> When ( PlayCard { GameId = 1; Player = 0; Card = red 9 } )
    |> Expect [ CardPlayed { GameId = 1; Player = 0; Card = red 9 } ]

[<Fact>]
let ``Same value should be accepted``() =
    Given [ GameStarted { GameId = 1; PlayerCount = 4; FirstCard = red 3 } ]
    |> When ( PlayCard { GameId = 1; Player = 0; Card = yellow 3 } )
    |> Expect [ CardPlayed { GameId = 1; Player = 0; Card = yellow 3 } ]

[<Fact>]
let ``Different value and color should be rejected``() =
    Given [ GameStarted { GameId = 1; PlayerCount = 4; FirstCard = red 3 } ]
    |> When ( PlayCard { GameId = 1; Player = 0; Card = yellow 8 } )
    |> ExpectThrows<InvalidOperationException>

[<Fact>]
let ``First player should play at his turn``() =
    Given [ GameStarted { GameId = 1; PlayerCount = 4; FirstCard = red 3 } ]
    |> When ( PlayCard { GameId = 1; Player = 2; Card = green 3 } )
    |> ExpectThrows<InvalidOperationException>
