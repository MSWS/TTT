namespace SpecialRoundAPI.Configs;

public record SilentRoundConfig : SpecialRoundConfig {
  public override float Weight { get; init; } = 0.5f;
}