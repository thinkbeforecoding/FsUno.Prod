[<AutoOpen>]
module Builders

open Deck

let red n = Digit(digit n, Red)
let green n = Digit(digit n, Green)
let blue n = Digit(digit n, Blue)
let yellow n = Digit(digit n, Yellow)
