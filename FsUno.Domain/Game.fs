module Game

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


let gameId =
    function
    | StartGame c -> c.GameId
    | PlayCard c -> c.GameId
    
type Event =
    | GameStarted of GameStartedEvent
    | CardPlayed of CardPlayedEvent
    | PlayerPlayedAtWrongTurn of PlayerPlayedAtWrongTurn 
    
and GameStartedEvent = {
    GameId: GameId
    PlayerCount: int
    FirstCard: Card }

and CardPlayedEvent = {
    GameId: GameId
    Player: int
    Card: Card }

and PlayerPlayedAtWrongTurn = {
    GameId: GameId
    Player: int
    Card: Card }

// A type representing current player turn
// All operation should be done inside the module

type Turn = (*player*)int * (*playerCount*)int

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Turn =
    let empty = (0,1)
    let start count = (0, count)
    let next (player, count) = (player + 1) % count, count
    let isNot p (current, _) = p <> current

type State = {
    GameAlreadyStarted: bool
    Player: Turn
    TopCard: Card }

let empty = {
    GameAlreadyStarted = false
    Player = Turn.empty
    TopCard = Digit(digit 0,Red) }

// Operations on the DiscardPile aggregate

let color = function
    | Digit(_,c) -> c
    | KickBack c -> c

let sameColor c1 c2 = color c1 = color c2
let (|SameColor|_|) (c1,c2) = if sameColor c1 c2 then Some (color c1) else None
let (|SameValue|_|) = function
    | Digit(n1,_), Digit(n2,_) when n1 = n2 -> Some()
    | KickBack _, KickBack _ -> Some()
    | _ -> None

let startGame (command: StartGame) state =
    if command.PlayerCount <= 2 then invalidArg "playerCount" "You should be at least 3 players"
    if state.GameAlreadyStarted then invalidOp "You cannot start game twice"

    [ GameStarted { GameId = command.GameId
                    PlayerCount = command.PlayerCount
                    FirstCard = command.FirstCard } ]

let playCard (command: PlayCard) state =
    if state.Player |> Turn.isNot command.Player then 
        [ PlayerPlayedAtWrongTurn { GameId = command.GameId
                                    Player = command.Player
                                    Card = command.Card } ]
    else
        match command.Card, state.TopCard with
        | SameColor _ 
        | SameValue ->            
            [ CardPlayed { GameId = command.GameId
                           Player = command.Player
                           Card = command.Card } ]
        | _ -> invalidOp "Play same color or same value !"

// Map commands to aggregates operations

let handle =
    function
    | StartGame command -> startGame command
    | PlayCard command -> playCard command














// Applies state changes for events

let apply state =
    function
    | GameStarted event -> 
        { GameAlreadyStarted = true
          Player = Turn.start event.PlayerCount
          TopCard = event.FirstCard }

    | CardPlayed event ->
        { state with
            Player = state.Player |> Turn.next 
            TopCard = event.Card }

// Replays all events from start to get current state

let replay events = List.fold apply empty events


