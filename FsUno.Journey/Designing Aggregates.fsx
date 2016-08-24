(**
*2014-08-11*

# Designing Aggregates, Algebra

This evening, I start to feel that things won't move much as long as I have only one Aggregate.

With several Aggregates we'll have to add Process and Process Managers to orchestrate the
reactions to events, and ensure the global consistency of the system.

We'll also have to manage projections over several aggregates. And make the infrastructure code
a bit more generic to not wire it directly to the only aggregate.

But before this happens: Design !

## The Game Aggregate

This one is already partly implemented and represents the enforcement of rules on the discard pile.

----

A discussion on Jabbr with [@rbartelink](https://twitter.com/@rbartelink) interrupted me in the
process for something awesome !

## Algebra

The first question was whether to change the ``empty`` member and make it a ``static member``, ditto the ``apply`` function.

The second was whether to try to hide ``empty`` and ``apply`` completly in a ``replay`` function.

``empty`` as a static member feels quite natural, but I had a problem with ``apply``. Its signature
seems to validate this change, but putting it before ``handle`` seems wrong.

The problem is that when reading an Aggregate, the first important thing is the decision
in the ``handle`` method, and changes in ``apply`` are always implemented later. Changing
this would break the flow.

But it's possible to implement it further in F# with a type extension: *)
(*** hide ***)
type Command = | DoSomething
type Event = | SomethingHappened
module FirstAttempt = 
(** *)
    type State = { CurrentState: string }
        with
        static member empty = { CurrentState = "" }

    let handle command state =
        /// ...
        [ SomethingHappened ]

    type State with
        static member apply state = function
            | SomethingHappened -> { state with CurrentState = "new state" }

(**
The second question started with this sample code:
*)
        static member build = List.fold State.apply  
        static member replay = State.build State.empty 

(**
It would be possible to write this but it's not part of the Aggregate, let's see why.

### Monoids

Do you remember how a [Monoid](http://en.wikipedia.org/wiki/Monoid) is defined ?

A monoid is a triple (S,+,n) where 

* ``S`` is a set
* ``+`` is closed on the set (the signature would be S -> S -> S)
* ``+`` is associative ( x + y + z = (x + y) + z = x + (y + z) )
* ``n`` is the [neutral(aka identity)](http://en.wikipedia.org/wiki/Identity_element) element ( x + n = n + x = x)

Most of mathematical structures are defined using sets (here `S`), functions (here `+`) and specific set items
(here `n`).

### Aggregates

Aggregates are not monoids, but you can see that it could be defined using the same
kind of primary element:

A quadruple (State, handle, apply, empty) with the rules and signatures you know. ``handle`` and ``apply``
are not closed on the set but the composition is not bad.

If we alias ``apply`` as ``+`` we can write:
*)
    let (+) = State.apply

    let finalState = State.empty + SomethingHappened + SomethingHappened + SomethingHappened

(**
Makes sense, right ?

The only change that I would propose is to change ``empty`` to ``initial`` because it's the **initial** state:
*)
    type State with
        static member initial = { CurrentState = "initial state"}

    let finalState' = State.initial + SomethingHappened + SomethingHappened

(**
Hiding ``apply`` and ``initial`` would seriously reduce the composability of all this.

### Higher order functions

``replay`` can actually be seen as a high order function built using ``initial`` and ``apply``.
To build it directly from the ``State`` type we can use 
[statically resolved type parameters](http://msdn.microsoft.com/en-us/library/dd548046.aspx),
you can hover over the ``replay`` definition below to see the type constraints:
*)
    let inline replay events =
        let initial = (^S: (static member initial: ^S) ()) 
        let apply s = (^S: (static member apply: ^S -> (^E -> ^S)) s)
        List.fold apply initial events
(**
Ok, it's a bit more complicated but you can check the signature by hovering the function, and
it can be used on any type that has ``initial`` and ``apply`` members that adhere to the expected signature.
And the inference still works from the return type, so any hint to the compiler indicating
the specific Aggregate ``State`` type in use means there is no need for further type annotations.
*)
    let state : State =
        [ SomethingHappened; SomethingHappened]
        |> replay

(** So where are we now ?
Better vocabulary with ``initial``, better understanding about the Aggregate *algebra*, and
a high order function to use it.

Now we can see how we'll implement more generic command handlers.

Good evening !
*)