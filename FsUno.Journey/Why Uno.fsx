(**
# Why Uno

*tldr; This is not about the 'groking your domain' part of DDD. Here, the domain
is already clear and defined by precise rules.
I don't want to bloat the journey with questioning what the business is.
Study [EventStorming](http://ziobrando.blogspot.fr/2013/11/introducing-event-storming.html#.U-dALerlrX4) for this.*

## Choosing a sample domain

Choosing a sample domain is not a simple thing. I already made several attempts
to select one in the past and failed miserably.

### The case of the fake simplistic domain

It's hardly a domain at all, but it's easy to implement. It seems good at first
to teach newcomers, but they rapidly spot that it's a toy domain that won't match
their domain complexity. Experts will also point all the places where your model is
weak in explaining important concepts.

Failure.

### The case of the real business domain

Here, the domain is a real world example, like finance, shipping, hotel business and
so on. People agree that it's interesting because it goes into real problems.

**But then they start to argue about the business choices of the model.**
What happens when there is no product left to ship ? How do you deal with an overbooking ?

This are legitimate questions that need to be asked when building a Business Plan, and
it will drift from the goal here: *How to design and implement Functional Event Sourcing*.

There are loads of people (including me) talking about this subject. Obviously Eric Evan's
[Domain Driven Design](http://www.domaindrivendesign.org/) book, and especially parts
about Ubiquitus language and knowledge crunching, but also the more recent technique
of [EventStorming](https://plus.google.com/communities/113258571348605620818) created by
Alberto Brandolini and other DDD practitioners.

### So what ?

Choosing a game has a real advantage here: **The rules already exist**.

No need to discover them from the mind of Domain Experts, they're here in the leaflet.

Is it a toy domain because it's a game ? Not really because when rules are rich enough
to create interesting cases, it will match a real domain in a lot of aspects.

I choosed Uno because:

* Most people already know the rules
* Concurrency matters due to interruptions
* It contains several aggregates
* It's sufficiently complex (with the proposed variant) to be interesting
* It's not too complex to be boring

There still will be a lot of design decision along the way, because even if the
rule are already written, we still have to choose how to represent them in the system.

And I'll write about these design decisions in this log.
*)