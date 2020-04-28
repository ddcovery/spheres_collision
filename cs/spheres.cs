using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Diagnostics;

namespace collisions_sharp
{

  public struct Sphere
  {
    readonly public float x;
    readonly public float y;
    readonly public float z;
    readonly public float r;

    public Sphere(float x, float y, float z, float r)
    {
      this.x = x;
      this.y = y;
      this.z = z;
      this.r = r;
    }

  }

  public static class SphereExtension
  {
    public static bool CollidesWith(this Sphere a, Sphere b)
    {
      float d_x = b.x - a.x;
      float d_y = b.y - a.y;
      float d_z = b.z - a.z;
      float a_r = b.r + a.r;
      return d_x * d_x + d_y * d_y + d_z * d_z < a_r * a_r;
    }

  }

  class Program
  {
    static void Main(string[] args)
    {
      Sphere[] spheres = readFile("../data/spheres.dat");
      crono("O(n2)", () =>
      {
        for (int i = 0; i < spheres.Length; i++)
        {
          for (int j = i + 1; j < spheres.Length; j++)
          {
            if (spheres[i].CollidesWith(spheres[j]))
            {
              Console.WriteLine("{0} collides with {1}", i, j);
            }
          }
        }
      });
      Console.ReadKey();
    }


    static void crono(string title, Action method)
    {
      Console.WriteLine(title);
      var sw = new Stopwatch();
      sw.Start();
      try
      {
        method();
      }
      finally
      {
        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);
      }
    }

    static Sphere[] readFile(string file)
    {
      var sphereList = new List<Sphere>();
      using (var f = new StreamReader(file))
      {
        for (var i = 0; i < 100000; i++)
        {
          float x = float.Parse(f.ReadLine(), CultureInfo.InvariantCulture);
          float y = float.Parse(f.ReadLine(), CultureInfo.InvariantCulture);
          float z = float.Parse(f.ReadLine(), CultureInfo.InvariantCulture);
          float r = float.Parse(f.ReadLine(), CultureInfo.InvariantCulture);
          sphereList.Add(new Sphere(x, y, z, r));
        }
      }

      return sphereList.ToArray();
    }
  }

}