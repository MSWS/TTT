namespace TTT.CS2.Utils;

public static class TextCompass {
  /// <summary>
  ///   Builds a compass line with at most one character for each of N, E, S, W.
  ///   0° = North, 90° = East, angles increase clockwise.
  /// </summary>
  /// <param name="fov">Field of view in degrees [0..360].</param>
  /// <param name="width">Output width in characters.</param>
  /// <param name="direction">Facing direction in degrees.</param>
  /// <param name="filler">Filler character for empty slots.</param>
  /// <param name="targetDir"></param>
  public static string GenerateCompass(float fov, int width, float direction,
    char filler = '·', float? targetDir = null) {
    if (width <= 0) return string.Empty;

    fov       = Math.Clamp(fov, 0.001f, 360f);
    direction = Normalize(direction);

    var buf                                = new char[width];
    for (var i = 0; i < width; i++) buf[i] = filler;

    var start      = direction - fov / 2f; // left edge of view
    var degPerChar = fov / width;

    PlaceIfVisible('N', 0f);
    PlaceIfVisible('E', 90f);
    PlaceIfVisible('S', 180f);
    PlaceIfVisible('W', 270f);
    if (targetDir.HasValue) PlaceIfVisible('X', targetDir.Value);

    return new string(buf);

    void PlaceIfVisible(char c, float cardinalAngle) {
      var delta = ForwardDelta(start, cardinalAngle); // [0..360)
      if (delta < 0f || delta >= fov) return;         // outside view

      // Map degrees to nearest character cell
      var idx               = (int)MathF.Round(delta / degPerChar);
      if (idx < 0) idx      = 0;
      if (idx >= width) idx = width - 1;

      // Nudge left/right to avoid collisions when possible
      if (buf[idx] == filler) {
        buf[idx] = c;
        return;
      }

      var maxRadius = Math.Max(idx, width - 1 - idx);
      for (var r = 1; r <= maxRadius; r++) {
        var left = idx - r;
        if (left >= 0 && buf[left] == filler) {
          buf[left] = c;
          return;
        }

        var right = idx + r;
        if (right < width && buf[right] == filler) {
          buf[right] = c;
          return;
        }
      }

      // If no space, overwrite the original cell as a last resort
      buf[idx] = c;
    }
  }

  private static float Normalize(float angle) {
    angle %= 360f;
    return angle < 0 ? angle + 360f : angle;
  }

  // Delta moving forward from start to target, wrapped to [0..360)
  private static float ForwardDelta(float start, float target) {
    var s = Normalize(start);
    var t = Normalize(target);
    var d = t - s;
    return d < 0 ? d + 360f : d;
  }
}