using System;

namespace collisions.algorithms
{
  /// <summary>
  /// Associates to each sphere a numeric interval of integers [min, max] such than 
  /// 2 spheres CAN only intersect if their numerics intervals also intersect.
  /// We call to each of this integers a partition
  /// * Minimum possible partition is 0.
  /// * Maximum possible partition depends on the spheres.
  /// </summary>
  internal class Partitioner
  {
    // The minimum partitionable x
    float x_min;
    // The maximum partitionable x 
    float x_max;
    // The size of each partition
    float size;

    int count;

    bool noSpheres = false;

    public Partitioner(Sphere[] spheres)
    {
      init(spheres);
    }

    /// <summary>The number of partitions</summary>
    public int getCount() => count;
    //1+getValuePartitionN(this.x_max);


    /// <summary>Wich partitions the sphere is located into.</summary>
    /// <returns>The interval [min, max] of partitions where spere is located</returns>
    public Interval getSpherePartitionsInterval(Sphere sphere) =>
      new Interval(getValuePartitionN(sphere.x - sphere.r), getValuePartitionN(sphere.x + sphere.r));



    private void init(in Sphere[] spheres)
    {
      if (spheres.Length == 0)
      {
        this.noSpheres = true;
        this.count = 1;
      }
      else
      {
        float x_min = spheres[0].x;
        float x_max = spheres[0].x;
        float r_sum = spheres[0].r;
        for (var n = 1; n < spheres.Length; n++)
        {
          if (spheres[n].x < x_min) x_min = spheres[n].x;
          if (spheres[n].x > x_max) x_max = spheres[n].x;
          r_sum += spheres[n].r;
        }

        float avg = (r_sum * 2) / spheres.Length;
        this.x_min = x_min;
        this.x_max = x_max;
        this.size = 1.1f * avg; 

        this.count = 1 + (int)Math.Ceiling((x_max - x_min) / size);
      }
    }

    private int getValuePartitionN(float x)
    {
      return noSpheres ? 0 : (int)((x - x_min) / size);
    }
  }


}