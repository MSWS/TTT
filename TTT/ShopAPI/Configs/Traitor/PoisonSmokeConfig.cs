namespace ShopAPI.Configs.Traitor;

public record PoisonSmokeConfig : ShopItemConfig {
  public override int Price { get; init; } = 30;

  public string Weapon { get; init; } = "smoke";

  public float SmokeRadius { get; init; } = 180;

  public PoisonConfig PoisonConfig { get; init; } =
    new() { DamagePerTick = 20, TotalDamage = 500 };
}