namespace TTT.Game;

public record TTTConfig {
  public RoleConfig RoleCfg { get; init; } = new();
  public RoundConfig RoundCfg { get; init; } = new();
  public BalanceConfig BalanceCfg { get; init; } = new();

  public record BalanceConfig {
    public Func<int, int> TraitorCount { get; init; } =
      p => (int)Math.Ceiling((p - 1) / 5f);

    public Func<int, int> DetectiveCount { get; init; } =
      p => (int)Math.Floor(p / 8f);

    public Func<int, int> InnocentCount { get; init; } = p
      => p - (int)Math.Ceiling((p - 1) / 5f) - (int)Math.Floor(p / 8f);
  }

  public record RoleConfig {
    public int TraitorHealth { get; init; } = 100;
    public int DetectiveHealth { get; init; } = 100;
    public int InnocentHealth { get; init; } = 100;
    public int TraitorArmor { get; init; } = 100;
    public int DetectiveArmor { get; init; } = 100;
    public int InnocentArmor { get; init; } = 100;

    public string[]? TraitorWeapons { get; init; } = ["knife", "pistol"];

    public string[]? DetectiveWeapons { get; init; } = [
      "taser", "pistol", "rifle"
    ];

    public string[]? InnocentWeapons { get; init; } = ["knife", "pistol"];

    public bool StripWeaponsPriorToEquipping { get; init; } = false;
  }

  public record RoundConfig {
    public TimeSpan CountDownDuration { get; init; } = TimeSpan.FromSeconds(10);
    public TimeSpan TimeBetweenRounds { get; init; } = TimeSpan.FromSeconds(5);
    public int MinimumPlayers { get; init; } = 2;

    public virtual TimeSpan RoundDuration(int players) {
      return TimeSpan.FromSeconds(players switch {
        < 4  => 60,
        < 6  => 90,
        < 8  => 120,
        < 10 => 150,
        < 14 => 180,
        < 20 => 210,
        < 30 => 240,
        _    => 300
      });
    }
  }
}