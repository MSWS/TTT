namespace ShopAPI.Configs;

public record CamoConfig : ShopItemConfig {
  public override int Price { get; init; } = 65;
  public float CamoVisibility { get; init; } = 0.6f;
}