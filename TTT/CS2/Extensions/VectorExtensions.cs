using CounterStrikeSharp.API.Modules.Utils;

namespace TTT.CS2.Extensions;

public static class VectorExtensions {
  public static double Distance(this Vector vec, Vector other) {
    return Math.Sqrt(vec.DistanceSquared(other));
  }

  public static double DistanceSquared(this Vector vec, Vector other) {
    return (vec.X - other.X) * (vec.X - other.X) + (vec.Y - other.Y) * (vec.Y - other.Y)
      + (vec.Z - other.Z) * (vec.Z - other.Z);
  }
}