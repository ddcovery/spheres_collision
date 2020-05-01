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
Let **N** be the number of spheres, ***intertersect*** method will be called  N * (N-1) / 2 = (N²-N)/2 times. *Thats a **O(N²)** cost*

## The Partition algorithm

### The algorithm

The strategy is to generate **"potential" intersection groups of spheres** (our "**partitions**"). 

* Each sphere belongs to one or more partitions.
* Two spheres CAN intersect if they share, at least, one partition.

As a corolay

*  Two spheres will not intersect if they don't share any partition.

A possible algorithm based on partitions:

```
var intersections := [];
var partitioner := Partitioner(spheres);

foreach sphere_a in spheres
  foreach partition in partitioner.validSpherePartitions(sphere_a)
    foreach sphere_b in partition.spheres
      if(intersect(sphere_a, sphere_b))
        intersections.add( Tuple(sphere_a, sphere_b ));
      end
    end

    partition.add(sphere_a);
  end
end
```

### The Complexity

The number of times we call "intersect" method is **O(S * P * M)** where
* **S** = Number of Spheres
* **P** = Number of partitions where each sphere is located
* **M** = Number of Spheres in each partition.

There are 2 extreme situations:
1. **Best**: There is no collision. Implies **M=1** and the complexity will be O(P * S)
2. **Worst**: All spheres collide with each other. Implies **M=S** and the complexity will be O(P * S²)

**S** and **M** depends on the data set (the spheres itself): we can do nothing to change the number of spheres and the numer of collisions. 

We only can work to improve the value of **P** tryint to obtain **P=1** (Each sphere is located in 1 partition) and this is what the ```Partitioner``` has to achive

### Linear partition size

Partitioner must analyze all spheres and decide the best way to group them. The first poposal (used originally in my "javascript" solver) is to represent each sphere as a "segment" in the X axis (We can also choose Y or Z).

We define the "segment" of an sphere as **[x-r, x+r]** where:
* **x** is the X coordinate of the central point of the sphere
* **r** is the sphere radius
* **2*r** is the segment "size": max-min = (x+r) - (x-r) = 2 * r

The idea is to project each possible X value to an Integer that represents a partition (p<sub>x</sub>):
* p<sub>x</sub> = f( x )
* p<sub>x</sub> is Integer
* x1 < x2 → p<sub>x1</sub> ≤ p<sub>x2</sub> 

An sphere segment will be projected to an interval of integer numbers:

* [p<sub>min</sub> , p<sub>max</sub>] = [f(x-r) , f(x+r)])
* p<sub>min</sub> ≤ p<sub>min</sub>

A possible partition function can be p<sub>x</sub> = [ (x - x<sub>min</sub>) / size ) ] where

* x<sub>min</sub> is the minimum x coordinate of any sphere segment.
* size is (x<sub>max</sub> - x<sub>min</sub>) /  Average<sub>spheres</sub>( 2 * r )

This function requires a previous spheres analisis to deduce x<sub>min</sub>, x<sub>max</sub> and Average.  The cost of this analisis is N (where N is the number of spheres)

¿why?

* We suposse that spheres diameter (2*radius) has a very low variance (all radius are very similar).
* We suposse that spheres x coordinate follows an uniform distribution too.

An "small" adjustment is to multiply average by 1.1 factor (supossing a constant variance):
* Using average as partition size causes a lot of spheres to occupy 3 partitions instead 2 partitions. 
* Using a value bigger than average minimizes this problem.

### Adaptative partition size

Previous solution suposes an uniform distribution of the spheres in the X axis.








      

