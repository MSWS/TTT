namespace SpecialRoundAPI;

public record VanillaRoundConfig : SpecialRoundConfig {
  public override float Weight { get; init; } = 0.2f;
}