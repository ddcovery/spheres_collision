using System.Collections.Generic;
using System.Linq;
using System;

namespace collisions.algorithms
{

  public static partial class Algorithms
  {
   
    public static List<Tuple<int, int>> findCollisionsUsingPartitions(Sphere[] spheres)
    {
      var collisions = new List<Tuple<int, int>>();
      var partitioner = new Partitioner(spheres);
      var partitions = Enumerable.Range(0, partitioner.getCount()).Select((x) => new List<Sphere>()).ToArray();

      foreach (var sphere in spheres)
      {
        foreach (var collided in detectAndInsert(sphere, partitions, partitioner))
        {
          collisions.Add(new Tuple<int, int>(sphere.id, collided));
        }
      }

      return collisions;
    }
    static private IEnumerable<int> detectAndInsert(Sphere sphere, List<Sphere>[] partitions, Partitioner partitioner)
    {
      var collided = new HashSet<int>();
      Interval interval = partitioner.getSpherePartitionsInterval(sphere);

      for (var partIdx = interval.min; partIdx <= interval.max; partIdx++)
      {
        var partSpheres = partitions[partIdx];
        foreach (var partSphere in partSpheres)
        {
          if (sphere.CollidesWith(partSphere) && !collided.Contains(partSphere.id))
          {
            // If the same sphere is present in more than 1 partition, we detect the same intersection repetidely.  The Map is used to avoid duplicates.
            collided.Add(partSphere.id);
          }
        }
        // After checking, we add the sphere to the partition 
        partSpheres.Add(sphere);
      }
      return collided;
    }
  }


}