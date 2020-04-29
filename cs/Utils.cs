using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Diagnostics;
namespace collisions
{
  public static class Utils
  {
    public static List<Tuple<int, int>> crono(string title, Func<List<Tuple<int, int>>> method)
    {
      Console.WriteLine(title);
      var sw = new Stopwatch();
      sw.Start();
      var result = method();
      sw.Stop();
      result.ForEach(collision =>
        Console.WriteLine("  {0} collides with {1}", collision.Item1, collision.Item2)
      );
      Console.WriteLine("{0}ms", sw.ElapsedMilliseconds);
      return result;
    }

    public static Sphere[] readFile(string file)
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
          sphereList.Add(new Sphere(i, x, y, z, r));
        }
      }

      return sphereList.ToArray();
    }


  }
}