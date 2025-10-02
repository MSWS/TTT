namespace ShopAPI.Configs.Traitor;

// TODO: Support this config
public record C4Config : ShopItemConfig {
  public override int Price { get; init; } = 140;
  public string Weapon { get; init; } = "c4";
  public int MaxC4PerRound { get; init; } = 0;
  public int MaxC4AtOnce { get; init; } = 1;
  public float Power { get; init; } = 100f;
  public TimeSpan FuseTime { get; init; } = TimeSpan.FromSeconds(30);
  public bool FriendlyFire { get; init; } = false;
}