using System.Collections.Generic;
using System.Linq;
using System;

namespace collisions.algorithms
{

  public static partial class Algorithms
  {

    public static List<Tuple<int, int>> findCollisionsUsingPartitions2(Sphere[] spheres)
    {
      var collisions = new List<Tuple<int, int>>();
      var partitioner = new Partitioner(spheres);
      var partitions = Enumerable.Range(0, partitioner.getCount()).Select((x) => new List<Sphere>()).ToArray();

      foreach (var sphere_a in spheres)
      {
        var added = new HashSet<int>();
        foreach (Sphere sphere_b in partitions.ListPotential(sphere_a, partitioner))
        {
          if (sphere_a.CollidesWith(sphere_b) && !added.Contains(sphere_b.id))
          {
            added.Add(sphere_b.id);
            collisions.Add(new Tuple<int, int>(sphere_a.id, sphere_b_id));
          }
        }
        partitions.AddSphere(sphere_a, partitioner);
      }

      return collisions;
    }


    static private IEnumerable<Sphere> ListPotential(this List<Sphere>[] partitions, Sphere sphere, Partitioner partitioner)
    {
      Interval interval = partitioner.getSpherePartitionsInterval(sphere);
      for (int partIdx = interval.min; partIdx <= interval.max; partIdx++)
      {
        foreach (Sphere sphere_b in partitions[partIdx])
        {
          yield return sphere_b;
        }
      }


    }
    static private void AddSphere(this List<Sphere>[] partitions, Sphere sphere, Partitioner partitioner)
    {
      Interval interval = partitioner.getSpherePartitionsInterval(sphere);

      for (var partIdx = interval.min; partIdx <= interval.max; partIdx++)
      {
        partitions[partIdx].Add(sphere);
      }
    }

  }


}