module FsUno.Tests.``When playing a kickback``

open FsUno.Domain
open Deck
open Game

open Specifications
open Xunit

let gameId = GameId 1

[<Fact>]
let ``it should change game direction``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 8 }
            CardPlayed { GameId = gameId; Player = 0; Card = green 8; NextPlayer = 1 } ]
    |> When (PlayCard { GameId = gameId; Player = 1; Card = KickBack Green}  )
    |> Expect [ CardPlayed { GameId = gameId; Player = 1; Card = KickBack Green; NextPlayer = 0}
                DirectionChanged { GameId = gameId; Direction = CounterClockWise } ]

[<Fact>]
let ``after a game change, it should change game direction again``() =
    Given [ GameStarted { GameId = gameId; PlayerCount = 4; FirstCard = red 8 }
            CardPlayed { GameId = gameId; Player = 0; Card = green 8; NextPlayer = 1 }
            CardPlayed { GameId = gameId; Player = 1; Card = KickBack Green; NextPlayer = 0}
            DirectionChanged { GameId = gameId; Direction = CounterClockWise }  ]
    |> When (PlayCard { GameId = gameId; Player = 0; Card = KickBack Blue }  )
    |> Expect [ CardPlayed { GameId = gameId; Player = 0; Card = KickBack Blue; NextPlayer = 1}
                DirectionChanged { GameId = gameId; Direction = ClockWise } ]                