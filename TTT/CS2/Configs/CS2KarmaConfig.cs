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
    "css_ttt_karma_dbstring", "Database connection string for Karma storage",
    "Data Source=karma.db", ConVarFlags.FCVAR_NONE);

  public static readonly FakeConVar<int> CV_MIN_KARMA = new("css_ttt_karma_min",
    "Minimum possible Karma value", 0, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(0, 1000));

  public static readonly FakeConVar<int> CV_DEFAULT_KARMA = new(
    "css_ttt_karma_default", "Default Karma value for new players", 50,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 1000));

  public static readonly FakeConVar<string> CV_LOW_KARMA_COMMAND = new(
    "css_ttt_karma_low_command",
    "Command executed when a player falls below the Karma threshold (use {0} for player name)",
    "css_ban #{0} 4320 Your karma is too low!");

  public static readonly FakeConVar<int> CV_KARMA_TIMEOUT_THRESHOLD = new(
    "css_ttt_karma_timeout_threshold",
    "Minimum Karma to avoid punishment or timeout effects", 20,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 1000));

  public static readonly FakeConVar<int> CV_KARMA_ROUND_TIMEOUT = new(
    "css_ttt_karma_round_timeout", "Number of rounds a Karma penalty persists",
    4, ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 100));

  public void Dispose() { }

  public void Start() { }

  public void Start(BasePlugin? plugin) {
    ArgumentNullException.ThrowIfNull(plugin, nameof(plugin));
    plugin.RegisterFakeConVars(this);
  }

  public Task<KarmaConfig?> Load() {
    var cfg = new KarmaConfig {
      DbString              = CV_DB_STRING.Value,
      MinKarma              = CV_MIN_KARMA.Value,
      DefaultKarma          = CV_DEFAULT_KARMA.Value,
      CommandUponLowKarma   = CV_LOW_KARMA_COMMAND.Value,
      KarmaTimeoutThreshold = CV_KARMA_TIMEOUT_THRESHOLD.Value,
      KarmaRoundTimeout     = CV_KARMA_ROUND_TIMEOUT.Value
    };

    return Task.FromResult<KarmaConfig?>(cfg);
  }
}