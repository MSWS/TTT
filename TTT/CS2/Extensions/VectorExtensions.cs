using CounterStrikeSharp.API.Modules.Utils;

namespace TTT.CS2.Extensions;

public static class VectorExtensions {
  public static float Distance(this Vector vec, Vector other) {
    return MathF.Sqrt(vec.DistanceSquared(other));
  }

  public static float DistanceSquared(this Vector vec, Vector other) {
    return (vec.X - other.X) * (vec.X - other.X)
      + (vec.Y - other.Y) * (vec.Y - other.Y)
      + (vec.Z - other.Z) * (vec.Z - other.Z);
  }

  public static Vector? Clone(this Vector? vec) {
    if (vec == null) return null;
    return new Vector(vec.X, vec.Y, vec.Z);
  }

  public static Vector Normalized(this Vector vec) {
    var length = vec.Length();
    if (length == 0) return new Vector(0, 0, 0);
    return new Vector(vec.X / length, vec.Y / length, vec.Z / length);
  }

  public static Vector ToLength(this Vector vec, float length) {
    var normalized = vec.Normalized();
    return normalized * length;
  }

  public static QAngle? Clone(this QAngle? angle) {
    if (angle == null) return null;
    return new QAngle(angle.X, angle.Y, angle.Z);
  }

  public static Vector ToForward(this QAngle angle) {
    var pitch = angle.X * (Math.PI / 180.0);
    var yaw   = angle.Y * (Math.PI / 180.0);

    var cosPitch = Math.Cos(pitch);

    return new Vector((float)(Math.Cos(yaw) * cosPitch),
      (float)(Math.Sin(yaw) * cosPitch), (float)-Math.Sin(pitch));
  }
}