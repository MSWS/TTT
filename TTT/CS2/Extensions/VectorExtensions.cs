using CounterStrikeSharp.API.Modules.Utils;

namespace TTT.CS2.Extensions;

public static class VectorExtensions {
  public static double Distance(this Vector vec, Vector other) {
    return Math.Sqrt(vec.DistanceSquared(other));
  }

  public static double DistanceSquared(this Vector vec, Vector other) {
    return Math.Pow(vec.X - other.X, 2) + Math.Pow(vec.Y - other.Y, 2)
      + Math.Pow(vec.Z - other.Z, 2);
  }
}