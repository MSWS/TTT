namespace SpecialRoundAPI.Configs;

public record PistolRoundConfig : SpecialRoundConfig {
  public override float Weight { get; init; } = 0.75f;
}