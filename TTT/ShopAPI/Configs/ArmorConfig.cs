namespace ShopAPI.Configs;

public record ArmorConfig : ShopItemConfig {
  public override int Price { get; init; } = 80;
  public int Armor { get; init; } = 100;
  public bool Helmet { get; init; } = true;
}