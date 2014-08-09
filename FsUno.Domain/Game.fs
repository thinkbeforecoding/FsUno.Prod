module FsUno.Domain.Game

open Deck

[<Struct>]
type GameId =
    val id: int
    new (id) = {id = id}
    override this.ToString() = string this.id
    
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


let gameId = function
    | StartGame c -> c.GameId
    | PlayCard c -> c.GameId
    
type Event =
    | GameStarted of GameStartedEvent
    | CardPlayed of CardPlayedEvent
    | PlayerPlayedAtWrongTurn of PlayerPlayedWrong 
    | PlayerPlayedWrongCard of PlayerPlayedWrong
    | DirectionChanged of DirectionChanged
    
and GameStartedEvent = {
    GameId: GameId
    PlayerCount: int
    FirstCard: Card }

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


// A type representing current player turn
// All operation should be done inside the module

type Turn = (*player*)int * (*playerCount*)int

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Turn =
    let empty = (0,1)
    let start count = (0, count)

    let next direction (player, count) = 
        match direction with
        | ClockWise -> (player + 1) % count, count
        | CounterClockWise -> (player + count - 1) % count, count  // the + count is here to avoid having negative result

    let skip direction = next direction >> next direction

    let isNot p (current, _) = p <> current

    let set player (_, count) =
        if player < 0 || player >= count then 
            invalidArg "player" "The player value should be between 0 and player count"
        player, count

    let player (current, _) = current

module Direction =
    let reverse = function
        | ClockWise -> CounterClockWise
        | CounterClockWise -> ClockWise

type State = {
    GameAlreadyStarted: bool
    Player: Turn
    TopCard: Card 
    Direction: Direction}

let empty = {
    GameAlreadyStarted = false
    Player = Turn.empty
    TopCard = Digit(digit 0,Red) 
    Direction = ClockWise}

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
    | _ -> None

let startGame (command: StartGame) state =
    if command.PlayerCount <= 2 then invalidArg "playerCount" "There should be at least 3 players"
    if state.GameAlreadyStarted then invalidOp "The game cannot be started more than once"

    [ yield GameStarted { GameId = command.GameId
                          PlayerCount = command.PlayerCount
                          FirstCard = command.FirstCard }
      match command.FirstCard with
      | KickBack _ ->
        yield DirectionChanged { GameId = command.GameId 
                                 Direction = CounterClockWise }
      | _ -> () ]

let playCard (command: PlayCard) state =
    if state.Player |> Turn.isNot command.Player then 
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
                let newDirection = Direction.reverse state.Direction
                let nextPlayer = 
                    state.Player 
                    |> Turn.next newDirection
                    |> Turn.player

                [ cardPlayed nextPlayer
                  DirectionChanged { GameId = command.GameId
                                     Direction = newDirection } ]
            | Skip _ ->
                let nextPlayer =
                    state.Player
                    |> Turn.skip state.Direction
                    |> Turn.player

                [ cardPlayed nextPlayer ]
            | _ -> 
                let nextPlayer =
                    state.Player
                    |> Turn.next state.Direction
                    |> Turn.player
                [ cardPlayed nextPlayer ]
        
        | _ -> [ PlayerPlayedWrongCard { GameId = command.GameId; Player = command.Player; Card = command.Card} ] 

// Map commands to aggregates operations

let handle = function
    | StartGame command -> startGame command
    | PlayCard command -> playCard command

// Applies state changes for events

let apply state = function
    | GameStarted event -> 
        { GameAlreadyStarted = true
          Player = Turn.start event.PlayerCount
          TopCard = event.FirstCard 
          Direction = ClockWise }
    | CardPlayed event ->
        { state with
            Player = state.Player |> Turn.set event.NextPlayer
            TopCard = event.Card }
    | DirectionChanged event ->
        { state with
            Direction = event.Direction}
    | PlayerPlayedAtWrongTurn _
    | PlayerPlayedWrongCard _ -> state 

// Replays all events from start to get current state

let replay events = List.fold apply empty events
