module FsUno.Tests.``When starting game``

open Xunit
open System
open Specifications
open Deck
open Game


[<Fact>]
let ``Started game should be started``() =
    Given []
    |> When ( StartGame { GameId = 1; PlayerCount = 4; FirstCard = red 3 } )
    |> Expect [ GameStarted { GameId = 1; PlayerCount = 4; FirstCard = red 3 } ]

[<Fact>]
let ``0 players should be rejected``() =
    Given []
    |> When ( StartGame { GameId = 1; PlayerCount = 0; FirstCard = red 3 } )
    |> ExpectThrows<ArgumentException>


[<Fact>]
let ``Game should not be started twice``() =
    Given [GameStarted { GameId = 1; PlayerCount = 4; FirstCard = red 3 } ]
    |> When ( StartGame { GameId = 1; PlayerCount = 4; FirstCard = red 2 } )
    |> ExpectThrows<InvalidOperationException>
