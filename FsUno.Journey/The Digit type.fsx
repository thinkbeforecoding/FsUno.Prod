(** 
#The Digit type and domain constraints

*tldr; Model domain constraints inside types.
Use model constraints or exceptions for invariants that should be ensured 
at all time by the system.*

*2016-08-24*

## Primitive obsession

There a several ways to model digits for this game, the simplest one is
to use an integer.

*)
(*** hide ***)
open System
type Color = 
    | Red
    | Green
    | Blue
    | Yellow
module WithIntegers =    
(*** ***)
    type Digit = int
    type Card = 
        | Digit of Digit * Color
      //| ... other card types

(** It is quite flacky modeling. 
   What would this represent ? *)
    (10,Blue) 
    (Int32.MaxValue, Yellow)
    (-3, Green)
(** It's such a recurrent smell it has a name: [Primitive Obsession](https://en.wikipedia.org/wiki/Design_smell).

The main problem is that the code will have to check the validity of the value in various places. 
Any missed check leading to potential bugs and in some cases, security risks.

The solution is to restrain the range of values from start, so that we don't have 
to think about it later.

## Wrapping type

My second option was to wrap the value in a more specific type to avoid bogus values:
*)
(*** hide ***)
module WrappingType =
(*** ***)
    type Digit = private Digit of int

    let digit n =
        if n < 0 || n > 9 then
            invalidArg "n" "Digit should be between 0 and 9."
        Digit n
(**
Now, once a digit value has been constructed, I'm sure it's valid.

Far better.

It's no real problem to use exception here. Functions that work on `Digit` can safely trust
it has been build correctly, so no exception will be raised except in case of bugs. But in
this case, you don't want to go any further - Stop the Press ! - and if you have plugged 
a bug reporter - and you should - you'll receive a notification. Then fix the bug and release.

It's fine to raise exceptions for things that should not occure at all.

The other place where it could happen is at the system boundary: input fields, APIs.

For input fields, you should provide a field that restrict the user to enter valid input. In
the digit case, a dropdown selector or even better, a list of clickable cards. It's a good way
to guide the user into providing valid input. 

Another way is to provide a text field and using a try parse function:
*)
    type Result<'t,'e> =
        | Ok of 't
        | Failure of 'e

    let tryParse input =
        match Int32.TryParse input with
        | false, _ -> Failure "The value should be an integer"
        | true, n ->
            if n < 0 || n > 9 then
                Failure "The value should be between 0 and 9"
            else
                Ok (Digit n)
(**
Any way, even if you propose a restricted set of values, never assume the client sent a valid input.
Always check the value at input point using the `digit` constructor function.
*)        

(**
## Even more restrictive

In the case of Uno, the value of the card is almost never used for its value, cards just have
to be of equal value. It could as well be drawings like `Square`, `Circle`, `Star`...

In this case the type would probably modeled as: *)
(*** hide ***)
module Drawings =
(*** ***)
    type Drawing =
        | Square
        | Circle
        | Star
    //  | ... other drawings
(**
This type works perfectly for equality. Enough for me.

And it adds a futher constraint: 
    I cannot not represent at compile time an invalid value.

The previous version allowed me to write `digit -3`, which will fail at runtime, but will compile.
So I'll need unit tests to validate the `digit` constructor function. And hope that no one will do
something foolish in the code that will result in `digit n` where n doen's pass the check.

With the following version, it's just not possible: *)
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
(**
There is a single place where I'll need validation. When parsing it on the boundary, 
which is usually done inside deserialization:
*)
(*** hide ***)
type Result<'t,'e> = Ok of 't | Failure of 'e
(*** ***)
let cases = 
    [ Zero; One; Two;   Three;  Four
      Five; Six; Seven; Height; Nine ]
    |> List.map (fun d -> sprintf "%A" d, d)
    |> Map.ofList
    
let tryParse input =
    match Map.tryFind input cases with
    | Some digit -> Ok digit
    | None -> Failure "Unknown digit"

(** We can the try: *)
tryParse "Seven" // returns Ok Seven
tryParse "Bogus" // returns Failure "Unknown digit"

(**
The only place where the actual value is needed is for score, where a simple function is needed:
*)

let score digit =
    match digit with
    | Zero   -> 0
    | One    -> 1
    | Two    -> 2
    | Three  -> 3
    | Four   -> 4
    | Five   -> 5
    | Six    -> 6
    | Seven  -> 7
    | Height -> 8
    | Nine   -> 9
(**
The pattern matching is a total match, so we're sure we missed no case.

Of course, for bigger enumerations, like value between 1 and 100, the wrapping type is probably
a better option. *)