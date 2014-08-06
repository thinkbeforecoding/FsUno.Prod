module Deck


type Color = 
    | Red
    | Green
    | Blue
    | Yellow

type Card =
    | Digit of Value:int * Color:Color
    | KickBack of Color: Color

type Direction =
    | ClockWise
    | CounterClockWise
