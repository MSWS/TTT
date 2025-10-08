using TTT.API.Player;

namespace TTT.Karma;

public record KarmaConfig {
  public string DbString { get; init; } = "Data Source=karma.db";

  public int MinKarma { get; init; } = 0;
  public int DefaultKarma { get; init; } = 50;

  public string CommandUponLowKarma { get; init; } = "karmaban {0} Bad Player!";

  public int KarmaTimeoutThreshold { get; init; } = 20;
  public int KarmaRoundTimeout { get; init; } = 4;

  public TimeSpan KarmaWarningWindow { get; init; } = TimeSpan.FromDays(1);

  public int MaxKarma(IPlayer? player) { return 100; }
}