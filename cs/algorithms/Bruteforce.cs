using System.Collections.Generic;
using System.Linq;
using System;

namespace collisions.algorithms
{

  public static partial class Algorithms
  {
    public static List<Tuple<int, int>> findCollisionsUsingBruteforce(in Sphere[] spheres)
    {
      var collision = new List<Tuple<int, int>>();
      for (int i = 0; i < spheres.Length; i++)
      {
        for (int j = i + 1; j < spheres.Length; j++)
        {
          if (spheres[i].CollidesWith(spheres[j]))
          {
            collision.Add(new Tuple<int, int>(i, j));
          }
        }
      }
      return collision;
    }

  }
}
