using TTT.API.Player;

namespace TTT.Karma;

public record KarmaConfig {
  public string DbString { get; init; } = "Data Source=karma.db";

  public virtual int MinKarma => 0;
  public virtual int DefaultKarma => 50;
  public virtual int MaxKarma(IPlayer player) { return 100; }
  public virtual string CommandUponLowKarma => "karmaban {0} Bad Player!";
}