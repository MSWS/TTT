namespace SpecialRoundAPI.Configs;

public record VanillaRoundConfig : SpecialRoundConfig {
  public override float Weight { get; init; } = 0.5f;
}