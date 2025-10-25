namespace SpecialRoundAPI;

public abstract record SpecialRoundConfig {
  public abstract float Weight { get; init; }
}