namespace SpecialRoundAPI.Configs;

public record LowGravRoundConfig : SpecialRoundConfig {
  public override float Weight { get; init; } = 0.6f;
  public float GravityMultiplier { get; init; } = 0.5f;
}