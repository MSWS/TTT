namespace ShopAPI.Configs.Traitor;

public record PoisonShotsConfig : ShopItemConfig {
  public override int Price { get; init; } = 30;
  public TimeSpan TimeBetweenDamage { get; init; } = TimeSpan.FromSeconds(5);
  public int DamagePerTick { get; init; } = 5;
  public int TotalDamage { get; init; } = 50;
  public int TotalShots { get; init; } = 3;
}