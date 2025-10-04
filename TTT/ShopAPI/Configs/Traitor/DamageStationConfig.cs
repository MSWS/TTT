using System.Drawing;

namespace ShopAPI.Configs.Traitor;

public record DamageStationConfig : StationConfig {
  public override int HealthIncrements { get; init; } = -15;
  public override int TotalHealthGiven { get; init; } = -300;

  public override string UseSound { get; init; } = "sounds/buttons/blip2";

  public override Color GetColor(float health) {
    // 100% health = white
    // 10% health = red
    var r = 255;
    var g = (int)(255 * health);
    var b = (int)(255 * health);
    return Color.FromArgb(r, g, b);
  }
}