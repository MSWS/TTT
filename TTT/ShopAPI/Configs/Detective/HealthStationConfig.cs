using System.Drawing;

namespace ShopAPI.Configs.Detective;

public record HealthStationConfig : StationConfig {
  public virtual string UseSound { get; init; } = "sounds/buttons/blip1";

  public override int Price { get; init; } = 50;

  public override TimeSpan HealthInterval { get; init; } =
    TimeSpan.FromSeconds(2);

  public override int HealthIncrements { get; init; } = 10;

  public override Color GetColor(float health) {
    // 100% health = white
    // 10% health = blue
    var r = (int)(255 * health);
    var g = (int)(255 * health);
    var b = 255;
    return Color.FromArgb(r, g, b);
  }
}