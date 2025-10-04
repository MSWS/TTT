namespace ShopAPI.Configs;

public record CamoConfig : ShopItemConfig {
  public override int Price { get; init; } = 100;
  public float CamoVisibility { get; init; } = 0.4f;
}