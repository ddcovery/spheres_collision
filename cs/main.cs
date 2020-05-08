using System;
using System.Collections.Generic;
using System.Linq;
using collisions.algorithms;

namespace collisions
{

  class Program
  {
    static void Main(string[] args)
    {
      Sphere[] spheres = Utils.readFile("../data/spheres.dat");

      Utils.crono("Using partitions", () => Algorithms.findCollisionsUsingPartitions(spheres), 10);
      Utils.crono("Using partitions 2", () => Algorithms.findCollisionsUsingPartitions2(spheres), 10);
      Utils.crono("Using brute force", () => Algorithms.findCollisionsUsingBruteforce(spheres));

      Console.ReadKey();
    }


  }
}
