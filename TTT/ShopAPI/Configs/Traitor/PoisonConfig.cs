namespace ShopAPI.Configs.Traitor;

public record PoisonConfig {
  public TimeSpan TimeBetweenDamage { get; init; } = TimeSpan.FromSeconds(1.5);
  public int DamagePerTick { get; init; } = 5;
  public int TotalDamage { get; init; } = 60;

  public string PoisonSound { get; init; } =
    "sounds/player/player_damagebody_03";
}