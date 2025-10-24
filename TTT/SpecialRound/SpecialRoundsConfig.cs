namespace SpecialRound;

public record SpecialRoundsConfig {
  public int MinRoundsBetweenSpecial { get; init; } = 3;
  public int MinPlayersForSpecial { get; init; } = 8;
  public int MinRoundsAfterMapChange { get; init; } = 2;
  public float SpecialRoundChance { get; init; } = 0.2f;
}