(**
# Exception or Event ?

In the original FsUno code, I raised exceptions when the player played the wrong card, or played at wrong turn. I changed it to events in FsUno.Prod.

I still raise some exception when the player count is two or less when starting the game, or when the game is started twice.

The question is: When should we use events, and when should you use exceptions ?

## Prior reading

A few years back, I wrote a post on the subjet: [Business Errors are Just Ordinary Events](http://thinkbeforecoding.com/post/2009/12/10/Business-Errors-are-Just-Ordinary-Events)

And it makes a distinction betweens several cases...
## Broken invariants

This should never happen since the aggregate is here to preserve invariants.

easy, next...

## Invalid commands

There a two cases here:

### Command data is meaningless

Try to avoid it as much as you can by designing command types that cannot represent invalid case. When it's not possible, validate the data and raise an exception. The caller is responsible to not call the aggregate with such command bit still check it at the aggregate boundary to be sure.

This is the case when the game is requested to start with two players. The UI should not enable this scenario, but we still validate this and throw an exception in case a problem occures, or a malicious client tries to fool the system with an invalid command.

It could be possible to use a type for player count that cannot be constructed with less than two players. Checking player count inside the aggregate is then useless.

### The command leads to an invalid state

This should also not happen. The client is responsible to not send this command in the current application state. To be sure to avoid any problem, validate this and raise an exception.

This is the case when the game is requested to start again. The UI should not enable this scenario.

In all cases, exceptions are raised in cases that **should not** happen !

## Other cases

Other cases are part of the domain, and should raise an **event**. This cases are modeled as domain events representing things that can happen - even if rarely - in the domain.

This is the case when a player plays the wrong card. The rest of the process should be notified of this because the card should return in player's hand, and the player will get a penalty for not knowing the rule !

When a player plays at wrong turn the result depends on the kind of game - and UI - you want to implement:

* when the UI enforces player turns so that player can only take action when expected by the rules, a player should not be able to play a card out of turn. In the case, the aggregate could simply raise an exception, because this should simply not happen. No need to handle this.

* When the game is more wild, and the UI let player play whenever they want, players have no UI safety net and can play at wrong turn. The aggregate should emit an event because playing at wrong turn is now part of the game, and some reactions should follow.

This should make this kind of desing decision easier...
*)
