module ``When passing to next turn``

open Xunit
open FsUnit.Xunit
open Deck
open Game

[<Fact>]
let ``clockwise should be next one``() =
    Turn.start 4
    |> Turn.next ClockWise
    |> Turn.player
    |> should equal 1

[<Fact>]
let ``clockwith after last player should be first one``() =
    Turn.start 4
    |> Turn.set 3
    |> Turn.next ClockWise
    |> Turn.player
    |> should equal 0
