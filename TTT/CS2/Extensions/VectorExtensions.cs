using System.Numerics;
using CounterStrikeSharp.API.Modules.Utils;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace TTT.CS2.Extensions;

public static class VectorExtensions {
  public static float Distance(this Vector vec, Vector other) {
    return MathF.Sqrt(vec.DistanceSquared(other));
  }

  public static float DistanceSquared(this Vector? vec, Vector other) {
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

  public static Vector ToRight(this QAngle angle) {
    var pitch = angle.X * (Math.PI / 180.0);
    var yaw   = angle.Y * (Math.PI / 180.0);
    var roll  = angle.Z * (Math.PI / 180.0);

    var sinPitch = Math.Sin(pitch);
    var cosPitch = Math.Cos(pitch);
    var sinYaw   = Math.Sin(yaw);
    var cosYaw   = Math.Cos(yaw);
    var sinRoll  = Math.Sin(roll);
    var cosRoll  = Math.Cos(roll);

    return new Vector((float)(sinYaw * sinPitch * cosRoll - cosYaw * sinRoll),
      (float)(-cosYaw * sinPitch * cosRoll - sinYaw * sinRoll),
      (float)(cosPitch * -sinRoll));
  }

  public static Vector ToUp(this QAngle angle) {
    var pitch = angle.X * (Math.PI / 180.0);
    var yaw   = angle.Y * (Math.PI / 180.0);
    var roll  = angle.Z * (Math.PI / 180.0);

    var sinPitch = Math.Sin(pitch);
    var cosPitch = Math.Cos(pitch);
    var sinYaw   = Math.Sin(yaw);
    var cosYaw   = Math.Cos(yaw);
    var sinRoll  = Math.Sin(roll);
    var cosRoll  = Math.Cos(roll);

    return new Vector((float)(-cosYaw * sinPitch * cosRoll - sinYaw * sinRoll),
      (float)(-sinYaw * sinPitch * cosRoll + cosYaw * sinRoll),
      (float)(cosPitch * cosRoll));
  }

  public static Vector Lerp(this Vector from, Vector to, float t) {
    return new Vector(from.X + (to.X - from.X) * t,
      from.Y + (to.Y - from.Y) * t, from.Z + (to.Z - from.Z) * t);
  }

  public static Vector Slerp(this Vector current, Vector target,
    float maxDelta) {
    var toVector = target - current;
    var dist     = toVector.Length();
    if (dist <= maxDelta || dist == 0f) return target;

    return current + toVector / dist * maxDelta;
  }

  public static Vector toVector(this Vector3 vec) {
    return new Vector(vec.X, vec.Y, vec.Z);
  }
}