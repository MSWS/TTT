using System.Drawing;

namespace ShopAPI.Configs.Traitor;

public record PoisonShotsConfig : ShopItemConfig {
  public override int Price { get; init; } = 65;
  public int TotalShots { get; init; } = 5;
  public Color PoisonColor { get; init; } = Color.FromArgb(128, Color.Purple);
  public PoisonConfig PoisonConfig { get; init; } = new();
}