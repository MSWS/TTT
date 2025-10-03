using System.Drawing;

namespace ShopAPI.Configs;

public record BodyPaintConfig : ShopItemConfig {
  public override int Price { get; init; } = 60;
  public int MaxUses { get; init; } = 1;
  public Color ColorToApply { get; init; } = Color.GreenYellow;
}