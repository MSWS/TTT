using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using TTT.API;
using TTT.API.Storage;
using TTT.Karma;

namespace TTT.CS2.Configs;

public class CS2KarmaConfig : IStorage<KarmaConfig>, IPluginModule {
  public static readonly FakeConVar<string> CV_DB_STRING = new(
    "css_ttt_karma_db_string", "Database connection string for Karma storage",
    "Data Source=karma.db");

  public static readonly FakeConVar<int> CV_MIN_KARMA = new("css_ttt_karma_min",
    "Minimum possible Karma value; falling below executes the low-karma command",
    0, ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 1000));

  public static readonly FakeConVar<int> CV_DEFAULT_KARMA = new(
    "css_ttt_karma_default", "Default Karma assigned to new or reset players",
    50, ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 1000));

  public static readonly FakeConVar<string> CV_LOW_KARMA_COMMAND = new(
    "css_ttt_karma_low_command",
    "Command executed when a player's karma falls below the minimum (use {0} for player slot)",
    "css_ban #{0} 2880 Low Karma");

  public static readonly FakeConVar<int> CV_TIMEOUT_THRESHOLD = new(
    "css_ttt_karma_timeout_threshold",
    "Minimum Karma before timing a player out for KarmaRoundTimeout rounds", 20,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 1000));

  public static readonly FakeConVar<int> CV_ROUND_TIMEOUT = new(
    "css_ttt_karma_round_timeout",
    "Number of rounds a player is timed out for after falling below threshold",
    4, ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 100));

  public static readonly FakeConVar<int> CV_WARNING_WINDOW_HOURS = new(
    "css_ttt_karma_warning_window_hours",
    "Time window (in hours) preventing repeat warnings for low karma", 24,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(1, 168));

  // Karma deltas
  public static readonly FakeConVar<int> CV_INNO_ON_TRAITOR = new(
    "css_ttt_karma_inno_on_traitor",
    "Karma gained when Innocent kills a Traitor", 4, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(-50, 50));

  public static readonly FakeConVar<int> CV_TRAITOR_ON_DETECTIVE = new(
    "css_ttt_karma_traitor_on_detective",
    "Karma gained when Traitor kills a Detective", 1, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(-50, 50));

  public static readonly FakeConVar<int> CV_INNO_ON_INNO_VICTIM = new(
    "css_ttt_karma_inno_on_inno_victim",
    "Karma gained or lost when Innocent kills another Innocent who was a victim",
    -1, ConVarFlags.FCVAR_NONE, new RangeValidator<int>(-50, 50));

  public static readonly FakeConVar<int> CV_INNO_ON_INNO = new(
    "css_ttt_karma_inno_on_inno",
    "Karma lost when Innocent kills another Innocent", -5,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(-50, 50));

  public static readonly FakeConVar<int> CV_TRAITOR_ON_TRAITOR = new(
    "css_ttt_karma_traitor_on_traitor",
    "Karma lost when Traitor kills another Traitor", -6, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(-50, 50));

  public static readonly FakeConVar<int> CV_INNO_ON_DETECTIVE = new(
    "css_ttt_karma_inno_on_detective",
    "Karma lost when Innocent kills a Detective", -8, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(-50, 50));

  public static readonly FakeConVar<int> CV_KARMA_PER_ROUND = new(
    "css_ttt_karma_per_round",
    "Amount of karma a player will gain at the end of each round", 1,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 50));

  public static readonly FakeConVar<int> CV_KARMA_PER_ROUND_WIN = new(
    "css_ttt_karma_per_round_win",
    "Amount of karma a player will gain at the end of each round if their team won",
    1, ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 50));

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
      KarmaTimeoutThreshold = CV_TIMEOUT_THRESHOLD.Value,
      KarmaRoundTimeout     = CV_ROUND_TIMEOUT.Value,
      KarmaWarningWindow    = TimeSpan.FromHours(CV_WARNING_WINDOW_HOURS.Value),
      KarmaPerRound         = CV_KARMA_PER_ROUND.Value,
      KarmaPerRoundWin      = CV_KARMA_PER_ROUND_WIN.Value,
      INNO_ON_TRAITOR       = CV_INNO_ON_TRAITOR.Value,
      TRAITOR_ON_DETECTIVE  = CV_TRAITOR_ON_DETECTIVE.Value,
      INNO_ON_INNO_VICTIM   = CV_INNO_ON_INNO_VICTIM.Value,
      INNO_ON_INNO          = CV_INNO_ON_INNO.Value,
      TRAITOR_ON_TRAITOR    = CV_TRAITOR_ON_TRAITOR.Value,
      INNO_ON_DETECTIVE     = CV_INNO_ON_DETECTIVE.Value
    };

    return Task.FromResult<KarmaConfig?>(cfg);
  }
}