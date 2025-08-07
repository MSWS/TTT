using CounterStrikeSharp.API.Modules.Utils;

namespace TTT.CS2.Extensions;

public static class VectorExtensions {
  public static double Distance(this Vector vec, Vector other) {
    return Math.Sqrt(vec.DistanceSquared(other));
  }

  public static double DistanceSquared(this Vector vec, Vector other) {
    return vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z;
  }
}