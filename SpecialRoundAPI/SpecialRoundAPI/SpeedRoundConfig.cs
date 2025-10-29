namespace SpecialRoundAPI;

public record SpeedRoundConfig : SpecialRoundConfig {
  public override float Weight { get; init; } = 0.5f;

  public TimeSpan InitialSeconds { get; init; } = TimeSpan.FromSeconds(40);
  public TimeSpan SecondsPerKill { get; init; } = TimeSpan.FromSeconds(10);
}