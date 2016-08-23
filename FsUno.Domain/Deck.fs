module FsUno.Domain.Deck

type Digit =
    | Zero
    | One
    | Two
    | Three
    | Four
    | Five
    | Six
    | Seven
    | Height
    | Nine

type Color = 
    | Red
    | Green
    | Blue
    | Yellow

type Card =
    | Digit of Value:Digit * Color:Color
    | KickBack of Color: Color
    | Skip of Color:Color

type Direction =
    | ClockWise
    | CounterClockWise
