module FsUno.Domain.Deck

[<Struct>]
type Digit =
    val value: int
    new(value) =
        if value < 0 || value > 9 then 
            invalidArg "value" "A digit value should be from 0 to 9" 
        { value = value }
    override this.ToString() = string this.value 

type digit = Digit

type Color = 
    | Red
    | Green
    | Blue
    | Yellow

type Card =
    | Digit of Value:digit * Color:Color
    | KickBack of Color: Color
    | Skip of Color:Color

type Direction =
    | ClockWise
    | CounterClockWise

// Game events

type GameId = GameId of int

type GameStartedEvent = { 
    GameId: GameId
    PlayerCount: int
    FirstCard: Card 
    FirstPlayer: int }

type CardPlayed = {
    GameId: GameId
    Player: int
    Card: Card }

type CardPlayedEvent = {
    GameId: GameId
    Player: int
    Card: Card
    NextPlayer: int }

type PlayerPlayedAtWrongTurnEvent = {
    GameId: GameId
    Player: int
    Card: Card }

type PlayerPlayedWrongCardEvent = {
    GameId: GameId
    Player: int
    Card: Card }

type DirectionChangedEvent = {
    GameId: GameId
    Direction: Direction }