using TTT.API.Player;

namespace TTT.Karma;

public record KarmaConfig {
  public string DbString { get; init; }

  public int MinKarma => 0;
  public int DefaultKarma => 50;
  public int MaxKarma(IPlayer player) { return 100; }
}