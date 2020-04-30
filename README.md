# spheres_collision
A performance study about N spheres collision detection

# Introduction
Recently, I readed in Quora the question [Can an entire Game Engine be written in C#? Not only the game logic, but also the core engine, physics, rendering etc.?](https://www.quora.com/Can-an-entire-Game-Engine-be-written-in-C-Not-only-the-game-logic-but-also-the-core-engine-physics-rendering-etc/answer/Vladislav-Zorov)

Vladislav Zorov proposed as an answere an experiment: "As an experiment, I wrote some simple collision detection code in C# and C++, where I give it a list of 100,000 spheres in a file and it says which ones collide."

* [C#](https://pastebin.com/x7nQczbG) 
* [C++](https://pastebin.com/x7nQczbG)

The data file: [spheres.dat](https://www.dropbox.com/s/kq57aa2u28o082g/spheres.dat?dl=0)

I posted a comment, as a joke, telling that my javascript code (for the 100000 spheres) will beat easily the c++ code, demostrating that the Algorithm is more important that the nature of the language.

This is, basically, a study to propose both algorithmic strategies and its implementations in various programming languages.

## The spheres intersection

Let's start with a simpliest problem:  2 circles intersection.

If we express each circle as a center *(x,y)* and a radius *r*, how we can express a intersection condition?

First, we need to find a formal definition for the "intersect" concept:

> Two circles intersect when the **distance between its centers** is **smaller** than the **sum of their radii**.

That is:  

> **d<sub>ab</sub> ≤ r<sub>a</sub> + r<sub>b</sub>**

where 

* **d<sub>ab</sub>** is the distance between its centers.
* **r<sub>a</sub>** is the radius of circle A
* **r<sub>b</sub>** is the radius of circle B

If we use the coordinates of the centers **(x<sub>a</sub>,y<sub>a</sub>)** and **(x<sub>b</sub>,y<sub>b</sub>)**, the **distance between their centers** can be solved using Pythagorean theorem:

>  (x<sub>b</sub> - x<sub>a</sub>)² + (y<sub>b</sub> - y<sub>a</sub>)² = d²<sub>ab</sub>


The original intersection expression now can be expressed as

> **(x<sub>b</sub> - x<sub>a</sub>)² + (y<sub>b</sub> - y<sub>a</sub>)² ≤ (r<sub>a</sub> + r<sub>b</sub>)²**

For spheres, the expression is exactly de same, but adding a new dimension (An sphere is a 3-dimensional object):

> **(x<sub>b</sub> - x<sub>a</sub>)² + (y<sub>b</sub> - y<sub>a</sub>)² + (z<sub>b</sub> - z<sub>a</sub>)² ≤ (r<sub>a</sub> + r<sub>b</sub>)²**

The Intersect function, as a procedural programming language, will be:

```pascal
function intersect(a:Sphere, b:Sphere):boolean 
  var x_diff := b.x - a.x
  var y_diff := b.y - a.y
  var z_diff := b.z - a.x
  var r_sum := a.r + b.r

  return x_diff * x_diff  +  y_diff * y_diff  +  z_diff * z_diff  <=  r_sum * r_sum
end
```
## The Brute force Algorithm

Given a set of N spheres, a brute force Algorithm will check the intersection of all possible spheres pairs

```pascal
Var intersected := []
For sphere_a in spheres
  For sphere_b in spheres
    If( sphere_a != sphere_b and intersect(sphere_a, sphere_b) )      
      intersected.add( Pair(sphere_a, sphere_b) );
    End
  End
End
```

Upps, this generates 2 pairs of spheres for each intersection.  Let's correct it this way

```pascal
Var intersected := []
For a:=1 to count(spheres)-1
  For b:=a+1 to count(spheres)
    If intersect( spheres[a], spheres[b])
      intersected.add( Pair(spheres[a], spheres[b]) )
    End
  End
End
```
Let **N** be the number of spheres, ***intertersect*** method will be called  N * (N-1) / 2 = (N²-N)/2. *Thats a **O(N²)** cost*

## The Partition algorithm:  *O(n log(n) )*

O(n log n) algorithms are based, usually, in a partitioning strategy.
i.e.:  
> Quick Sort tries to partitionate dinamically the items in 3 sets:  {smaller elements}, {The pivot element}, {bigger elements}.  In each iteration the {smaller} and {bigger} sets are "re-partitioned" until we achieve empty sets.  
> The total number of sets required to acomplish the objective is, usually, an O(n log(n)) number, although in some cases  (the worst ones) it achieves a O(n²) number.

In our case, the strategy is to generate **"potential" intersection groups of spheres** (our "**partitions**"). 

* Each sphere belongs to one or more partitions.
* Two spheres intersect only if they share, at least, one partition.

As a corolay

*  Two spheres will not intersect if they don't share any partition.




