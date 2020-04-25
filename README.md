# Conflicted - The Stellaris Mod Manager
Conflicted is a mod manager for Stellaris (although I think with a little bit of work it could be used for other new Paradox games, as they seem to work very similarly).

## Work in Progress
I'm not happy with some parts of the mod manager's implementation. If anyone knows how to improve on these, tell me, or better yet, go ahead and show me. I'm sure I'll lose interest in developing this little project soon enough and I'd be happy if someone else wants to fork this repo and see what they can do with it.

### Multi-threading
I'm an amateur programmer. Multi-threading is scary and I don't understand ~~half~~ most of it. I tried to implement it as well as I could and it seems to work just fine for pre-caching purposes, without breaking the rest of the code. Still, I'd love to get some qualified feedback on the implementation.

### Element Interpretation
Currently I take every text file, go through it character by character and extract element objects from the text. It feels very error-prone, probably doesn't catch everything and it takes a lot of work to extend functionality to capture new types of elements. It was the least time-consuming solution I've found, however, since the only alternative I could think of was writing a sort-of mini compiler for Paradox's script files and that would have been a little overkill.
