using System.Drawing;

namespace ShopAPI.Configs;

public record BodyPaintConfig : ShopItemConfig {
  public override int Price { get; init; } = 40;
  public int MaxUses { get; init; } = 4;
  public Color ColorToApply { get; init; } = Color.GreenYellow;
}