(**
# More Algebra
*2014-09-05*

After a short one week vacation, getting back to FsUno was a bit difficult with work and kids back to school. Today, I'm stuck in bed with a flu, giving me some time to write a bit.

Just after writting the previous log entry, there was a confusion with reviewers... is this a Monoid or not ?

It looks like one, but is definitly not since the function ``apply`` is not closed on the set. 

But a simple structure like this **had** to have a name, and be studied by mathematicians !

## Wikipedia to the rescue

First of all, if ``apply`` is not an internal operation, it surely is external !

Found this:

[http://en.m.wikipedia.org/wiki/External_(mathematics)]

**Left external binary operation** and **Right external binary operation**. In the case of fold the signature is usually ``'Acc -> 'T -> 'Acc`` so it's a Right external binary operation !

* An external magma ( S, \* ) over R is a set S with an external binary operation. This satisfies r \* s in S for all s in S, r in R (external closure).
* An external semigroup ( S, \* ) over (R, .) is an external magma that satisfies (r1 . r2) \* s = r1 \* (r2 \* s) for all s in S, r1, r2 in R (externally associative).
* An external monoid ( S, \* ) over (R, .) is an external semigroup in which there exists 1 in R such that 1 \* s = s for all s in S (has external identity element).
 
Great !

Here's how it goes. Replace:

* R with the set of event lists

* . with the event list concatenation

* 1 with the empty event list

* S with the set of states

* \* with the apply function

And we have exactly an external monoid here !

A bit further:

* A dynamical system (T, S, Phi) is an external monoid (S, Phi) over the monoid (T, +).

## Dynamical system

**That** seems interesting !


*The dynamical system concept is a mathematical formalization for any fixed "rule" which describes the time dependence of a point's position in its ambient space.*

*The concept unifies very different types of such "rules" in mathematics: the different choices made for how time is measured and the special properties of the ambient space may give an idea of the vastness of the class of objects described by this concept.*

*Time can be measured by integers, by real or complex numbers or can be a more general algebraic object, losing the memory of its physical origin, and the ambient space may be simply a set, without the need of a smooth space-time structure defined on it.*

from [http://en.m.wikipedia.org/wiki/Dynamical_system_(definition)]

The fun thing is that in our case, time passing is not uniform like a real number, but **a list of events** which may not be spaced at regular intervals in time !

From there, we can find a bunch of theories studying those systems from various points of view. I did not sort all that out yet but there are surely interesting properties and invariants to find around there.

And the first interesting point is that we didn't get things wrong previously since the first state is called ``initial state`` !

The ``apply`` function is called the ``evolution function`` and could be named ``evolve`` in our case.

The set of states is called the **Phase Space** or the **State Space**.

And terms like flow and orbits appear... we can surely do something with it !

There are still many things to read on the subject but being backed by theory is surely a serious advantage !



*)
