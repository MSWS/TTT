namespace ShopAPI.Configs;

public record TaserConfig : ShopItemConfig {
  public override int Price { get; init; } = 120;
  public string Weapon { get; init; } = "taser";
}