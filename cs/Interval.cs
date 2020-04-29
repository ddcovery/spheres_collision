namespace collisions
{
  public struct Interval
  {
    public readonly int min;
    public readonly int max;
    public Interval(int min, int max)
    {
      this.min = min;
      this.max = max;
    }
  }
}