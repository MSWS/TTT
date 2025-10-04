using System.Drawing;

namespace ShopAPI.Configs.Detective;

public record HealthStationConfig : StationConfig {
  public override string UseSound { get; init; } = "sounds/buttons/blip1";

  public override Color GetColor(float health) {
    // 100% health = white
    // 10% health = green
    var r = (int)(255 * health);
    var g = 255;
    var b = (int)(255 * health);
    return Color.FromArgb(r, g, b);
  }
}