namespace ShopAPI.Configs.Traitor;

public record PoisonConfig {
  public TimeSpan TimeBetweenDamage { get; init; } = TimeSpan.FromSeconds(2);
  public int DamagePerTick { get; init; } = 5;
  public int TotalDamage { get; init; } = 50;

  public string PoisonSound { get; init; } =
    "sounds/player/player_damagebody_03";
}