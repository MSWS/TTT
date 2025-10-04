namespace ShopAPI.Configs.Detective;

public record StickersConfig : ShopItemConfig {
  public override int Price { get; init; } = 30;
}