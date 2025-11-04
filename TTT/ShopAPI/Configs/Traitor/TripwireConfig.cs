namespace ShopAPI.Configs.Traitor;

public record TripwireConfig : ShopItemConfig {
  public override int Price { get; init; } = 60;
  public int ExplosionPower { get; init; } = 100;
}