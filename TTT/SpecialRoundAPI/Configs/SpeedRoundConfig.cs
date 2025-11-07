namespace SpecialRoundAPI.Configs;

public record SpeedRoundConfig : SpecialRoundConfig {
  public override float Weight { get; init; } = 1;

  public TimeSpan InitialSeconds { get; init; } = TimeSpan.FromSeconds(40);
  public TimeSpan SecondsPerKill { get; init; } = TimeSpan.FromSeconds(8);
  public TimeSpan MaxTimeEver { get; init; } = TimeSpan.FromSeconds(90);
}