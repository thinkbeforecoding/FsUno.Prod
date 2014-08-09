module FsUno.Tests.``When passing to next turn``

open FsUno.Domain
open Deck
open Game

open FsUnit.Xunit
open Xunit

[<Fact>]
let ``clockwise should be next one``() =
    Turn.start 0 4
    |> Turn.next ClockWise
    |> Turn.player
    |> should equal 1

[<Fact>]
let ``clockwith after last player should be first one``() =
    Turn.start 3 4
    |> Turn.next ClockWise
    |> Turn.player
    |> should equal 0