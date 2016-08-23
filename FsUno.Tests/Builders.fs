[<AutoOpen>]
module FsUno.Tests.Builders

open FsUno.Domain
open Deck

let red n = Digit(n, Red)
let green n = Digit(n, Green)
let blue n = Digit(n, Blue)
let yellow n = Digit(n, Yellow)