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
    | Eight
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

// Game events

type GameId = GameId of int

type GameStartedEvent = { GameId: GameId; PlayerCount: int; FirstCard: Card; FirstPlayer: int }
type CardPlayedEvent = { GameId: GameId; Player: int; Card: Card; NextPlayer: int }
type PlayerPlayedAtWrongTurnEvent = { GameId: GameId; Player: int; Card: Card }
type PlayerPlayedWrongCardEvent = { GameId: GameId; Player: int; Card: Card }
type DirectionChangedEvent = { GameId: GameId; Direction: Direction }