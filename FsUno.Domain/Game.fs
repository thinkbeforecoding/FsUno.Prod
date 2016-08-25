module FsUno.Domain.Game

open Deck

type GameId = GameId of int

// Commands
    
type Command =
    | StartGame of StartGame
    | PlayCard of PlayCard

and StartGame = {
    GameId: GameId
    PlayerCount: int
    FirstCard: Card }

and PlayCard = {
    GameId: GameId
    Player: int
    Card: Card }

// Events

type Event =
    | GameStarted of GameStartedEvent
    | CardPlayed of CardPlayedEvent
    | PlayerPlayedAtWrongTurn of PlayerPlayedWrong 
    | PlayerPlayedWrongCard of PlayerPlayedWrong
    | DirectionChanged of DirectionChanged
    
and GameStartedEvent = {
    GameId: GameId
    PlayerCount: int
    FirstCard: Card 
    FirstPlayer: int }

and CardPlayedEvent = {
    GameId: GameId
    Player: int
    Card: Card
    NextPlayer: int }

and PlayerPlayedWrong = {
    GameId: GameId
    Player: int
    Card: Card }

and DirectionChanged = {
    GameId: GameId
    Direction: Direction }

// Type representing current player turn; All operations should be encapsulated

type Turn = { 
    Player: int
    PlayerCount:int
    Direction:Direction }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Turn =
    let empty = { Player= 0; PlayerCount = 1; Direction = ClockWise }
    let start player count = {Player = player; PlayerCount = count; Direction = ClockWise }

    let next turn = 
        match turn.Direction with
        | ClockWise ->  { turn with Player = (turn.Player + 1) % turn.PlayerCount }
        | CounterClockWise -> { turn with Player = (turn.Player + turn.PlayerCount - 1) % turn.PlayerCount } // the + count is here to avoid having negative result

    let skip = next >> next

    let reverse turn = 
        match turn.Direction with
        | ClockWise -> { turn with Direction = CounterClockWise }
        | CounterClockWise -> { turn with Direction = ClockWise }

    let setPlayer player turn =
        if player < 0 || player >= turn.PlayerCount then 
            invalidArg "player" "The player value should be between 0 and player count"
        { turn with Player = player }

    let setDirection direction turn =
        { turn with Turn.Direction = direction }

// State

type State = 
    | InitialState
    | Started of Started
    static member initial = InitialState
and Started =
    { Turn: Turn
      TopCard: Card }
    

// Operations on the Game aggregate

let color = function
    | Digit(_,c) -> c
    | KickBack c -> c
    | Skip c -> c

let sameColor c1 c2 = color c1 = color c2
let (|SameColor|_|) (c1,c2) = if sameColor c1 c2 then Some (color c1) else None
let (|SameValue|_|) = function
    | Digit(n1,_), Digit(n2,_) when n1 = n2 -> Some()
    | KickBack _, KickBack _ -> Some()
    | Skip _, Skip _ -> Some()
    | _ -> None

let startGame (command: StartGame) state =
    if command.PlayerCount <= 2 then invalidArg "playerCount" "There should be at least 3 players"
    match state with 
    | Started _ -> invalidOp "The game cannot be started more than once"
    | InitialState ->    
        let gameStarted firstPlayer = 
            GameStarted { GameId = command.GameId
                          PlayerCount = command.PlayerCount
                          FirstCard = command.FirstCard
                          FirstPlayer = firstPlayer }
     
        match command.FirstCard with
        | KickBack _ ->
            [ gameStarted 0 
              DirectionChanged { GameId = command.GameId; Direction = CounterClockWise } ]
        | Skip _ -> [ gameStarted 1 ]
        | _ -> [ gameStarted 0]

let playCard (command: PlayCard) = function
    | InitialState -> invalidOp "Game has not been started"
    | Started state ->
        if state.Turn.Player <> command.Player then 
            [ PlayerPlayedAtWrongTurn { GameId = command.GameId
                                        Player = command.Player
                                        Card = command.Card } ]
        else
            match command.Card, state.TopCard with
            | SameColor _ 
            | SameValue ->
                let cardPlayed nextPlayer = 
                    CardPlayed { GameId = command.GameId
                                 Player = command.Player
                                 Card = command.Card 
                                 NextPlayer = nextPlayer}
              
                match command.Card with
                | KickBack _ ->
                    let nextTurn = state.Turn |> Turn.reverse |> Turn.next

                    [ cardPlayed nextTurn.Player
                      DirectionChanged { GameId = command.GameId
                                         Direction = nextTurn.Direction } ]
                | Skip _ ->
                    let nextTurn = state.Turn |> Turn.skip

                    [ cardPlayed nextTurn.Player ]
                | _ -> 
                    let nextTurn = state.Turn |> Turn.next
                    [ cardPlayed nextTurn.Player ]
            | _ -> [ PlayerPlayedWrongCard { GameId = command.GameId; Player = command.Player; Card = command.Card} ] 

// Map commands to aggregate operations

let handle = function
    | StartGame command -> startGame command
    | PlayCard command -> playCard command

// Identify Aggregate instance to direct Commands to

let gameId = function
    | StartGame c -> c.GameId
    | PlayCard c -> c.GameId
    
// Applies State changes for events

type State with
    static member evolve state = function
        | GameStarted event -> 
            Started { 
              Turn = Turn.start event.FirstPlayer event.PlayerCount 
              TopCard = event.FirstCard }
        | CardPlayed event ->
            match state with
            | Started state ->
                Started { 
                    state with
                        Turn = state.Turn |> Turn.setPlayer event.NextPlayer
                        TopCard = event.Card }
            | _ -> invalidOp "Game should be started"
        | DirectionChanged event ->
            match state with
            | Started state ->
                Started {
                    state with 
                        Turn = state.Turn |> Turn.setDirection event.Direction }
            | _ -> invalidOp "Game should be started"
        | PlayerPlayedAtWrongTurn _ 
        | PlayerPlayedWrongCard _ -> 
            state
