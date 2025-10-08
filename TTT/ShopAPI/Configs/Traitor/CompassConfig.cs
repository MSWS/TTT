namespace ShopAPI.Configs.Traitor;

public record CompassConfig : ShopItemConfig {
  public override int Price { get; init; } = 70;
  public float MaxRange { get; init; } = 10000;
}