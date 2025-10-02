namespace ShopAPI.Configs;

public abstract record ShopItemConfig {
  public abstract int Price { get; init; }
}