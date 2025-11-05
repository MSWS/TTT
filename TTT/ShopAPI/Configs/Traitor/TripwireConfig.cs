namespace ShopAPI.Configs.Traitor;

public record TripwireConfig : ShopItemConfig {
  public override int Price { get; init; } = 60;
  public int ExplosionPower { get; init; } = 1000;
  public float FalloffDelay { get; init; } = 0.02f;
  public float FriendlyFireMultiplier { get; init; } = 0.5f;
  public bool FriendlyFireTriggers { get; init; } = true;
}