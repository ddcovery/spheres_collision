using System;

namespace collisions
{
  /// <summary>
  /// Associates to each sphere a numeric interval of integers [min, max] such than 
  /// 2 spheres CAN only intersect if their numerics intervals also intersect.
  /// We call to each of this integers a partition
  /// * Minimum possible partition is 0.
  /// * Maximum possible partition depends on the spheres.
  /// </summary>
  public class Partitioner
  {
    // Minimum X ocupied by some sphere
    private float minX;
    // Number of partitions
    private int partsCount;
    // Width in X axis of each partition
    private float partSize;

    public Partitioner(Sphere[] spheres)
    {
      calculatePartitionerParams(spheres);
    }

    /// <summary>The number of partitions</summary>
    public int getCount() =>
      partsCount;


    /// <summary>Wich partitions the sphere is located into.</summary>
    /// <returns>The interval [min, max] of partitions where spere is located</returns>
    public Interval getSpherePartitionsInterval(Sphere sphere) =>
      new Interval(value2partitionNumber(sphere.x - sphere.r), value2partitionNumber(sphere.x + sphere.r));



    private void calculatePartitionerParams(in Sphere[] spheres)
    {
      var min = spheres[0].x - spheres[0].r;
      var max = spheres[0].x + spheres[0].r;
      var r = spheres[0].r;
      for (int ix = 1; ix < spheres.Length; ix++)
      {
        var left = spheres[ix].x - spheres[ix].r;
        if (left < min)
        {
          min = left;
        }
        var right = spheres[ix].x + spheres[ix].r;
        if (right > max)
        {
          max = right;
        }
        if (spheres[ix].r > r)
        {
          r = spheres[ix].r;
        }
      }
      // Bigger sphere width (r*2) is used as partition size.


      this.partsCount = Math.Max(1, (int)Math.Round((max - min) / (r * 2)));
      this.partSize = (int)Math.Round((max - min) / (float)partsCount);
      this.minX = min;
    }

    private int value2partitionNumber(float x)
    {
      return this.partSize != 0 ? (int)Math.Floor((x - this.minX) / this.partSize) : 0;
    }
  }


}