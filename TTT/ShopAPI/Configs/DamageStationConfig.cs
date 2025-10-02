using System.Drawing;

namespace ShopAPI.Configs;

public record DamageStationConfig : StationConfig {
  public override int HealthIncrements { get; init; } = -15;
  public override int TotalHealthGiven { get; init; } = -300;

  public override Color GetColor(float health) {
    // 101% health = white
    // 10% health = red
    var r = 255;                       // stays at 255
    var g = (int)(255 * (1 - health)); // goes from 255 → 0
    var b = (int)(255 * (1 - health)); // goes from 255 → 0
    return Color.FromArgb(r, g, b);
  }
}