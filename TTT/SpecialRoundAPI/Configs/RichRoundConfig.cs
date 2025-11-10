namespace SpecialRoundAPI.Configs;

public record RichRoundConfig : SpecialRoundConfig {
  public override float Weight { get; init; } = 0.75f;
  public float BonusCreditsMultiplier { get; init; } = 2.0f;
  public float AdditiveCreditsMultiplier { get; init; } = 3.0f;
}