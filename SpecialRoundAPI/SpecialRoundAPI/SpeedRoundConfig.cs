namespace SpecialRoundAPI;

public record SpeedRoundConfig : SpecialRoundConfig {
  public override float Weight { get; init; } = 0.4f;

  public TimeSpan InitialSeconds { get; init; } = TimeSpan.FromSeconds(60);
  public TimeSpan SecondsPerKill { get; init; } = TimeSpan.FromSeconds(10);
}