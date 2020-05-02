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

As we will see a little later, the different strategies try to minimize the number of times we have to verify if there is a collision between any 2 spheres. But before that, let's explain what it means that 2 spheres collide.

Let's start with a simpliest problem:  2 circles intersection.

If we express each circle as a center *(x,y)* and a radius *r*, how we can express a intersection condition?

First, we need to find a formal definition for the "intersect" concept:

> Two circles intersect (collide) when the **distance between its centers** is **smaller** than the **sum of their radii**.

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

For spheres, the expression is exactly de same, but adding a new dimension (sphere center is an (x,y,z) point):

> **(x<sub>b</sub> - x<sub>a</sub>)² + (y<sub>b</sub> - y<sub>a</sub>)² + (z<sub>b</sub> - z<sub>a</sub>)² ≤ (r<sub>a</sub> + r<sub>b</sub>)²**

The ```intersect``` function, as a procedural programming language, will be:

```pascal
function intersect(a:Sphere, b:Sphere):boolean 
  var x_diff := b.x - a.x
  var y_diff := b.y - a.y
  var z_diff := b.z - a.x
  var r_sum := a.r + b.r

  return x_diff * x_diff  +  y_diff * y_diff  +  z_diff * z_diff  <=  r_sum * r_sum
end
```

## The Brute Force Algorithm

Given a set of N spheres, a brute force strategy will check the intersection of all possible spheres pairs

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

Let **N** be the number of spheres, ***intertersect*** method will be called  *N * (N-1) / 2* = ***(N²-N)/2*** times. *Thats a **O(N²)** complexity*

## The Partition algorithm

### The algorithm

The strategy is to generate **"potential" intersection groups of spheres**. 

* Each sphere belongs to one or more groups.
* Two spheres CAN intersect if they share, at least, one group.

As a corolay

*  Two spheres will not intersect if they don't share any group.

A possible algorithm based on groups:

```pascal
Var collitions := [];
Var partitioner := new Partitioner(spheres);
For sphere_a in spheres
  For sphere_b in collidedSpheres(sphere_a, partitioner)
    collitions.add( Pair(sphere_a, sphere_b ));
  End
End

Function collidedSpheres(sphere, partitioner) as
  Var collided := []
  For group in partitioner.getSphereGroups(sphere)
    For sphere_b in group     
      If intersect(sphere, sphere_b) && !collided.contains(sphere_b)
        collided.add(sphere_b)
      End
    End
    group.add(sphere);
  End

  Return collided;
End
```
Note than:
* Initially, ``partitioner`` contains empty groups:  each sphere is added to a group AFTER checking possible collitions. For this reason, 2 spheres are not processed in "different" order  (When you process Sphere[1], Sphere[2] has not been processed yet and it is not present in any group).
* Because 2 spheres can share 2 or more groups, it is required to check  ``!collided.contains(sphere_b)``.  This is cleanly a performance Issue that must be solved in each concrete implementation (i.e:  Hash<int> that stores an unique "key" associated to the sphere 

### The Complexity

The number of times we call "intersect" method is **O(S × G<sub>s</sub> × S<sub>g</sub>)** where
* **S** = Number of Spheres
* **G<sub>s</sub>** = Number of Groups where each Sphere is located
* **S<sub>g</sub>** = Number of Spheres in each Group.

There are 2 extreme "ideal" situations:
1. **Best**: There is no collision. Implies **S<sub>g</sub>=1** and the complexity will be **O(G<sub>s</sub> × S)**
2. **Worst**: All spheres collide with each other. Each froup will contain all spheres. Thats **S<sub>g</sub>=S** and the complexity will be **O(G<sub>s</sub> × S²)**

**S** and **S<sub>g</sub>** depends on the data set (the spheres itself): we can do nothing to change the number of spheres and the numer of collisions. 

We only can work to improve the value of **G<sub>s</sub>** tryint to obtain **G<sub>s</sub>=1** (Each sphere is located in only 1 group) and this is what the ```Partitioner``` has to achive

> Partitioner must analyze all spheres to find (ideally):
> * The maximum number of not empty groups with the minimum number of spheres per group.
> * Each sphere must be present in the minimum number of groups
>

### Something suitable for comparing spheres

First, we must encounter some **behaviour of the sphere** that is comparable (between 2 spheres) than will help us to discard immediatelly a potential collision.

A proposal (used originally in my "javascript" solver) is to take the X axis projection of the sphere (we can also choose Y or Z).  This "projection" is a _segment_ **[x-r, x+r]** where:
* **x** is the X coordinate of the central point of the sphere
* **r** is the sphere radius
* The segment size is **2*r** that's the sphere **diameter**

Given 2 spheres **A** and **B** we can make it comparable with a **custom "<" operator** that means **"completly left"**)
* **A "<" B** if **x<sub>A</sub> + r<sub>A</sub> < x<sub>B</sub> - r<sub>B</sub>**

![alt text](blobs/sphA_completlyleft_sphB.svg?raw=true)

> Two spheres **can't collide** when **A is completly left B** or **B is completly left A** (A"<"B or B"<"A)

and it's corollary

> Two spheres **can collide** when (and only when) **A is not completly left B** and **B is not completly left A**  (!(A "<" B) and !(B "<" A))


### The partition  

We will project each possible **x** value to an Integer **p<sub>x</sub>** that represents a partition:
* p<sub>x</sub> = f( x )
* p<sub>x</sub> is Integer
* **x1 < x2 <big>→</big> p<sub>x1</sub> ≤ p<sub>x2</sub>**

An sphere segment will be projected to an interval of integer numbers:

* [p<sub>min</sub> , p<sub>max</sub>] = [f(x-r) , f(x+r)])
* p<sub>min</sub> **≤** p<sub>max</sub>

A possible partition function can be **p<sub>x</sub> = [ (x - x<sub>min</sub>) / size ) ]** where

* **x<sub>min</sub>** is the minimum x value contained in all segments.
* **size** is **Average<sub>spheres</sub>( 2 * r )**

This "partition" function generates partitions of the same "size". It works efficiently under some supossitions:

* Spheres radius **r** has a very low variance (all radius are very similar).
* Spheres **x** coordinate has an uniform distribution (All partitions contain a similar number of spheres).

An "small" adjustment is to multiply average by 1.1 factor (supossing a constant variance):
* Using average as partition size causes a lot of spheres to occupy 3 partitions instead 2. 
* Using a value bigger than average minimizes this problem.


> Summary:
> * **p<sub>x</sub> = [ (x - x<sub>min</sub>) / size ) ]**  (note: ***[ a ]*** is the integer part of ***a***)
> * **x<sub>min</sub>** is the minimum x coordinate of any sphere segment
> * **size** is **1.1 * AVERAGE<sub>spheres</sub>( 2 * r )**

We propose a class that calculates the partitioning parameters and provides functions to 
* The total number of partitions
* The partitions interval of an sphere.
The initializacion requires an spheres analisis to deduce **x<sub>min</sub>**, **size** and the total number of partitions.  The cost of this analisis is N (where N is the number of spheres).

```pascal
Class Partitioner
  // The minimum partitionable x
  Var x_min:Real
  // The maximum partitionable x 
  Var x_max:Real
  // The size of each partition
  Var size:Real

  // How many partitions are there.
  Public Function count():Integer
    Return partition_n( this.x_max )
  End
  // Sphere partitions interval [pmin, pmax]
  Public Function sphere_partitions_n(sphere: Sphere): Tuple<Integer,Integer>
    Var min:integer := partition_n(sphere.x - sphere.r)
    Var max:integer := partition_n(sphere.x + sphere.r)
    Return Tuple(min, max)
  end

  // Class initializer
  constructor(spheres: Sphere[])

    If count(spheres) == 0
      this.x_min = +Infinite
      this.size = +Infinite      
    Else
      Var x_min: real := spheres[1].x
      Var x_max: real := spheres[1].x
      Var r_sum: real := spheres[1].r
      For n:=2 To count(spheres)
        If sphere.x<x_min Then x_min:=sphere.x
        If sphere.x>x_max Then x_max:=sphere.x
        r_sum := r_sum + r
      End
      Var avg = ( r_sum * 2 ) / count(spheres);
      this.x_min := x_min
      this.x_max := x_max
      this.size := 1.1 * avg;
    End
  End 
  // Gets the partition number associated to an x value
  // remarks: Partition is 1 for the first one.
  Function partition_n(x:Real):Integer
    Return 1 + Integer( (x-x_min) / size )
  End
End
```
**Remarks**:  Partitioner requires, at least, 1 sphere to work

## The final algorithm

The algorithm based on partitions must me adapted to the proposed "partitioner" that treats partition as a number.

In the new version, we have to manage "manually" te partitions spheres using "dynamic" arrays

```pascal
Var intersections := [];
Var partitioner := Partitioner(spheres);
Var partitions :=[]
// Initialize partitions (Array of N empty arrays )
For p_n:=1 to partitioner.count()
  partitions.add( [] );
End
// 
For sphere_a in spheres
  Var sphereIntersections = [];
  Var p_interval := partitioner.sphere_partitions_n(sphere_a)
  For p_n:=p_interval[1] to p_interval[2]
    For sphere_b in partitions[p_n]
      If(intersect(sphere_a, sphere_b) && !sphereIntersections.contains(sphere_b))
        sphereIntersections.add(sphere_b);
      End
    End
    partitions[p_n].add(sphere_a);
  End
  Foreach sphere_b in sphereIntersections
    intersections.add( Tuple(sphere_a, sphere_b ));
  End
End
```





      

