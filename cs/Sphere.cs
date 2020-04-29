namespace collisions
{

  public struct Sphere
  {
    readonly public int id;
    readonly public float x;
    readonly public float y;
    readonly public float z;
    readonly public float r;

    public Sphere(int id, float x, float y, float z, float r)
    {
      this.id = id;
      this.x = x;
      this.y = y;
      this.z = z;
      this.r = r;
    }

  }

  public static class SphereExtension
  {
    public static bool CollidesWith(in this Sphere a, in Sphere b)
    {

      float d_x = b.x - a.x;
      float d_y = b.y - a.y;
      float d_z = b.z - a.z;
      float a_r = b.r + a.r;
      return d_x * d_x + d_y * d_y + d_z * d_z < a_r * a_r;
    }

  }
}