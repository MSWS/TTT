using TTT.API.Player;

namespace TTT.Karma;

public record KarmaConfig {
  public string DbString { get; init; } = "Data Source=karma.db";

  /// <summary>
  ///   The minimum amount of karma a player can have.
  ///   If a player's karma falls below this value, the CommandUponLowKarma
  ///   will be executed.
  /// </summary>
  public int MinKarma { get; init; }

  /// <summary>
  ///   The default amount of karma a player starts with.
  ///   Once a player falls below MinKarma, their karma will
  ///   also be reset to this value.
  /// </summary>
  public int DefaultKarma { get; init; } = 50;

  /// <summary>
  ///   The command to execute when a player's karma falls below MinKarma.
  ///   The first argument will be the player's slot.
  /// </summary>
  public string CommandUponLowKarma { get; init; } = "karmaban {0} Bad Player!";

  /// <summary>
  ///   The minimum threshold that a player's karma must reach
  ///   before timing them out for KarmaRoundTimeout rounds;
  /// </summary>
  public int KarmaTimeoutThreshold { get; init; } = 20;

  /// <summary>
  ///   The number of rounds a player will be timed out for
  ///   if their karma falls below KarmaTimeoutThreshold.
  /// </summary>
  public int KarmaRoundTimeout { get; init; } = 4;

  /// <summary>
  ///   The time window in which a player will receive a warning
  ///   if their karma falls below KarmaWarningThreshold.
  ///   If the player has already received a warning within this time window,
  ///   no warning will be sent.
  /// </summary>
  public TimeSpan KarmaWarningWindow { get; init; } = TimeSpan.FromDays(1);

  /// <summary>
  ///   Amount of karma a player will gain at the end of each round.
  /// </summary>
  public int KarmaPerRound { get; init; } = 3;

  public int KarmaPerRoundWin { get; init; } = 5;

  public int INNO_ON_TRAITOR { get; init; } = 5;
  public int TRAITOR_ON_DETECTIVE { get; init; } = 1;
  public int INNO_ON_INNO_VICTIM { get; init; } = -1;
  public int INNO_ON_INNO { get; init; } = -4;
  public int TRAITOR_ON_TRAITOR { get; init; } = -5;
  public int INNO_ON_DETECTIVE { get; init; } = -6;

  public int MaxKarma(IPlayer? player) { return 100; }
}