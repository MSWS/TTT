namespace ShopAPI.Configs.Traitor;

public record CompassConfig : ShopItemConfig {
  public override int Price { get; init; } = 60;
  public float MaxRange { get; init; } = 10000;
  public float CompassFOV { get; init; } = 120;
  public int CompassLength { get; init; } = 64;
}