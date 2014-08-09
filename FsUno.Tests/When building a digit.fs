module FsUno.Tests.``When building a digit``

open FsUno.Domain
open Deck

open FsUnit.Xunit
open System
open Xunit

[<Fact>]
let ``0 should be constructable``() =
    let zero = digit 0
    zero.value |> should equal 0

[<Fact>]
let ``9 should be constructable``() =
    let nine = digit 9
    nine.value |> should equal 9

[<Fact>]
let ``negative number should fail``() =
    (fun () -> digit -1 |> ignore)
    |> should throw typeof<ArgumentException>

[<Fact>]
let ``number greater that 9 should fail``() =
    (fun () -> digit 10 |> ignore)
    |> should throw typeof<ArgumentException>