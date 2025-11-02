namespace SpecialRoundAPI.Configs;

public record SuppressedRoundConfig : SpecialRoundConfig {
  public override float Weight { get; init; } = 0.3f;
}