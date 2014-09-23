(**
# Exception or Event ?

In the original FsUno code, I raised exceptions when the player played a wrong card, or played out of turn. I switched from exceptions to events in FsUno.Prod.

I still raise some exceptions: if the player count is two or less when starting the game, or if the game is started twice.

The question is: When should we use events, and when should we use exceptions ?

## Prior reading

A few years back, I wrote a post on the subject: [Business Errors are Just Ordinary Events](http://thinkbeforecoding.com/post/2009/12/10/Business-Errors-are-Just-Ordinary-Events)

And it makes a distinction betweens several cases...

###] Broken invariants

This should never happen since the very purpose of an aggregate is to preserve invariants.

easy; next...

## Invalid commands

There a two cases here:

#### Command data is meaningless

Try to avoid this as much as you can by designing command types that cannot represent invalid cases. Where that's not possible, validate the data and raise an exception. The caller is responsible for not calling the aggregate with such commands, but one should still check it at the aggregate boundary to be sure.

This situation applies when a game start is requested without at least two players. The UI should not permit this scenario, but we still validate this and throw an exception in case a problem occurs, or a malicious client tries to fool the system with an invalid command.

One could design a type to represent the player count that cannot possibly be constructed with less than two players. Checking player count inside the aggregate is then rendered pointless.

### The command leads to an invalid state

This should also not be permitted. The client is responsible for preventing the triggering of such a command in such an application state. To ensure any such problems are avoided, check for this condition and raise an exception if it is detected.

This situation applies if a game is requested to start again. The UI should prevent this scenario.

In each of the preceding situations, exceptions are raised in cases that **should not** happen !

## Other cases

Other cases are part of the domain, and should thus raise an **event**. These cases are modeled as domain events representing things that *can* happen - even if rarely - in the domain.

This is the case where a player plays an incorrect card. The rest of the process should be notified of this occurrence because the card should return to the player's hand, and the player will suffer a penalty for not knowing the rules !

When a player plays out of turn, the outcome depends on the kind of game - and UI - you want to implement:

* if the UI enforces player turns such that players can only take actions when expected by the rules, a player should not be able to play a card out of turn. In this case, the aggregate could simply raise an exception, because in this case such a command should simply not be possible to trigger. There is no need to handle this.

* if the game is more loose, and the UI lets players play whenever they want, players have no UI safety net and *can* play out of wrongturn. In this case, the aggregate should emit an event because playing out of turn is now part of the game, and some reactions should follow.

Hopefully the examples above will help one considering appropriate design approaches to such conundrums...
*)
