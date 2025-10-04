using System.Drawing;

namespace ShopAPI.Configs.Traitor;

public record PoisonShotsConfig : ShopItemConfig {
  public override int Price { get; init; } = 40;
  public int TotalShots { get; init; } = 3;
  public Color PoisonColor { get; init; } = Color.FromArgb(128, Color.Purple);
  public PoisonConfig PoisonConfig { get; init; } = new();
}