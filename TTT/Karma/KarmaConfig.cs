using TTT.API.Player;

namespace TTT.Karma;

public record KarmaConfig {
  public string DbString { get; init; }
  public int MaxKarma(IPlayer player) => 100;
  public int MinKarma => 0;
  public int DefaultKarma => 50;
}