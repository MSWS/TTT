using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using TTT.API;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Karma;

namespace TTT.CS2.Configs;

public class CS2KarmaConfig : IStorage<KarmaConfig>, IPluginModule {
  public static readonly FakeConVar<string> CV_DB_STRING = new(
    "css_ttt_karma_dbstring", "Connection string for the karma database",
    "Data Source=karma.db");

  public static readonly FakeConVar<int> CV_MIN_KARMA = new("css_ttt_karma_min",
    "Minimum allowed karma value", 0, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(0, 10000));

  public static readonly FakeConVar<int> CV_DEFAULT_KARMA = new(
    "css_ttt_karma_default", "Default karma assigned to new players", 50,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 10000));

  public static readonly FakeConVar<int> CV_MAX_KARMA = new("css_ttt_karma_max",
    "Maximum possible karma value for any player", 100, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(0, 10000));

  public static readonly FakeConVar<string> CV_COMMAND_UPON_LOW_KARMA = new(
    "css_ttt_karma_command_upon_low",
    "Command to execute when a player's karma goes below the minimum. {0} is replaced with the player's SteamID.",
    "css_ban #{0} 4320 Your karma is too low!");

  public void Dispose() { }

  public void Start() { }

  public void Start(BasePlugin? plugin) {
    ArgumentNullException.ThrowIfNull(plugin, nameof(plugin));
    plugin.RegisterFakeConVars(this);
  }

  public Task<KarmaConfig?> Load() {
    var cfg = new KarmaConfigInternal {
      DbString                 = CV_DB_STRING.Value,
      MinKarmaValue            = CV_MIN_KARMA.Value,
      DefaultValue             = CV_DEFAULT_KARMA.Value,
      MaxValue                 = CV_MAX_KARMA.Value,
      CommandUponLowKarmaValue = CV_COMMAND_UPON_LOW_KARMA.Value
    };

    return Task.FromResult<KarmaConfig?>(cfg);
  }

  // Wraps KarmaConfig with injected runtime values from ConVars
  private record KarmaConfigInternal : KarmaConfig {
    public int MinKarmaValue { get; init; }
    public int DefaultValue { get; init; }
    public int MaxValue { get; init; }
    public string CommandUponLowKarmaValue { get; init; } = string.Empty;

    public override int MinKarma => MinKarmaValue;
    public override int DefaultKarma => DefaultValue;

    public override string CommandUponLowKarma => CommandUponLowKarmaValue;
    public override int MaxKarma(IPlayer player) { return MaxValue; }
  }
}