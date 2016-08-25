(**
# Evolve vs Apply

*2016-08-25*

I definitly changed the name of the `apply` function to `evolve`.

The name `apply` carries to much mutability semantic, and is to much associated with replay.

I also renamed `replay` to `fold` for the same reason.

People tend to get scared of Event Sourcing because of this replay thing... I often hear

    Will side effects happen again ?

Using `fold` let people see it as side effects free. It's just a simple computation.

*)
