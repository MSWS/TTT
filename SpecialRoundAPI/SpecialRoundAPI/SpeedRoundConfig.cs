namespace SpecialRoundAPI;

public record SpeedRoundConfig : SpecialRoundConfig {
  public override float Weight { get; init; } = 0.6f;

  public TimeSpan InitialSeconds { get; init; } = TimeSpan.FromSeconds(40);
  public TimeSpan SecondsPerKill { get; init; } = TimeSpan.FromSeconds(10);
  public TimeSpan MaxTimeEver { get; init; } = TimeSpan.FromMinutes(1);
}