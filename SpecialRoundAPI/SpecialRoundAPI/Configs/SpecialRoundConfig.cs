namespace SpecialRoundAPI.Configs;

public abstract record SpecialRoundConfig {
  public abstract float Weight { get; init; }
}