using System;
using System.Collections.Generic;
using System.Linq;

namespace collisions
{

  class Program
  {
    static void Main(string[] args)
    {
      Sphere[] spheres = Utils.readFile("../data/spheres.dat");

      Utils.crono("Using partitions", () => detectUsingPartitions(spheres));
      Utils.crono("Using brute force", () => detectUsingBruteforce(spheres));

      Console.ReadKey();
    }
    static List<Tuple<int, int>> detectUsingBruteforce(in Sphere[] spheres)
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
    static List<Tuple<int, int>> detectUsingPartitions(Sphere[] spheres)
    {

      var collisions = new List<Tuple<int, int>>();
      var partitioner = new Partitioner(spheres);
      var partitions = Enumerable.Range(0, partitioner.getCount()).Select((x) => new List<Sphere>()).ToArray();

      foreach (var sphere in spheres)
      {
        detectAndInsert(sphere);
      }
     
      return collisions;

      void detectAndInsert(in Sphere sphere)
      {
        var intersected = new HashSet<int>();
        Interval interval = partitioner.getSpherePartitionsInterval(sphere);

        for (var partIdx = interval.min; partIdx <= interval.max; partIdx++)
        {
          var partitionSpheres = partitions[partIdx];
          foreach (var partitionSphere in partitionSpheres)
          {
            if (sphere.CollidesWith(partitionSphere) && !intersected.Contains(partitionSphere.id))
            {
              // If the same sphere is present in more than 1 partition, we detect the same intersection repetidely.  The Map is used to avoid duplicates.
              intersected.Add(partitionSphere.id);
            }
          }
          // After checking, we add the sphere to the partition 
          partitionSpheres.Add(sphere);
        }
        if (intersected.Count != 0)
        {
          foreach (var collided in intersected)
          {
            collisions.Add(new Tuple<int, int>(sphere.id, collided));
          }
        }
      }
    }

  }
}
