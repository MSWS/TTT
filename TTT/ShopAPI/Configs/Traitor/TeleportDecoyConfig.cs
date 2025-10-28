namespace ShopAPI.Configs.Traitor;

public record TeleportDecoyConfig : ShopItemConfig {
  public override int Price { get; init; } = 80;
}