namespace SpecialRoundAPI;

public record BhopRoundConfig : SpecialRoundConfig {
  public override float Weight { get; init; } = 0.2f;
}