namespace SpecialRound;

public record SpecialRoundsConfig {
  public int MinRoundsBetweenSpecial { get; init; } = 3;
  public int MinPlayersForSpecial { get; init; } = 5;
  public int MinRoundsAfterMapChange { get; init; } = 2;
  public float SpecialRoundChance { get; init; } = 0.2f;

  /// <summary>
  /// If a special round is started, the chance that another special round
  /// will start in conjunction with it. This check is run until it fails,
  /// or we run out of special rounds to start.
  /// </summary>
  public float MultiRoundChance { get; init; } = 0.33f;
}