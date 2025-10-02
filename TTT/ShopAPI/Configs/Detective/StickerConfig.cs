namespace ShopAPI.Configs;

public record StickerConfig : ShopItemConfig {
  public override int Price { get; init; } = 70;
}