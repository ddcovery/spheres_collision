# spheres_intersection
A performance study about N spheres intersection detection

# Introduction
Recently, I readed in Quora the question [Can an entire Game Engine be written in C#? Not only the game logic, but also the core engine, physics, rendering etc.?](https://www.quora.com/Can-an-entire-Game-Engine-be-written-in-C-Not-only-the-game-logic-but-also-the-core-engine-physics-rendering-etc/answer/Vladislav-Zorov)

Vladislav Zorov proposed as an answere an experiment:

"As an experiment, I wrote some simple collision detection code in C# and C++, where I give it a list of 100,000 spheres in a file and it says which ones collide."

The proposed algorithm was not the objective of the answere, only the comparation of the two languages performance (9516ms c# vs 1485ms C++)

The coded can be found on Pastebin:

* [C#](https://pastebin.com/x7nQczbG) 
* [C++](https://pastebin.com/x7nQczbG)

Here’s the data file: [spheres.dat](https://www.dropbox.com/s/kq57aa2u28o082g/spheres.dat?dl=0)

Proposed Algorithm is, basically, a O(n^2) (two loops comparing all spheres with all other spheres).

Oviously, the algorithm is more important that the nature of the language, and I proposed myself to write a simple Javascript code that will beat the c++ O(n^2) version.

This is, basically, an small study where I can place/compare algorithm and languages. 


