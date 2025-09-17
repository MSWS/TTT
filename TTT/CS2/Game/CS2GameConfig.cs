using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using TTT.API;
using TTT.API.Storage;
using TTT.CS2.Validators;
using TTT.Game;

namespace TTT.CS2.Game;

public class CS2GameConfig : IStorage<TTTConfig>, IPluginModule {
  public static readonly FakeConVar<int> CV_ROUND_COUNTDOWN = new(
    "css_ttt_round_countdown", "Time to wait before starting a round", 10,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(1, 60));

  public static readonly FakeConVar<int> CV_MINIMUM_PLAYERS = new(
    "css_ttt_minimum_players", "Minimum number of Players to start a round", 2,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(2, 64));

  public static readonly FakeConVar<int> CV_ROUND_DURATION_BASE = new(
    "css_ttt_round_duration_base", "Base round duration in seconds", 60,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(30, 600));

  public static readonly FakeConVar<int> CV_ROUND_DURATION_PER_PLAYER = new(
    "css_ttt_round_duration_per_player",
    "Additional round duration per player in seconds", 30,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 60));

  public static readonly FakeConVar<int> CV_ROUND_DURATION_MAX = new(
    "css_ttt_round_duration_max", "Maximum round duration in seconds", 300,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(60, 600));

  public static readonly FakeConVar<int> CV_TRAITOR_HEALTH = new(
    "css_ttt_rolehp_traitor",
    "Amount of health to give to traitors at start of round", 100,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(1, 1000));

  public static readonly FakeConVar<int> CV_DETECTIVE_HEALTH = new(
    "css_ttt_rolehp_detective",
    "Amount of health to give to detectives at start of round", 100,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(1, 1000));

  public static readonly FakeConVar<int> CV_INNOCENT_HEALTH = new(
    "css_ttt_rolehp_innocent",
    "Amount of health to give to innocents at start of round", 100,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(1, 1000));

  public static readonly FakeConVar<int> CV_TRAITOR_ARMOR = new(
    "css_ttt_rolearmor_traitor",
    "Amount of armor to give to traitors at start of round", 100,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 1000));

  public static readonly FakeConVar<int> CV_DETECTIVE_ARMOR = new(
    "css_ttt_rolearmor_detective",
    "Amount of armor to give to detectives at start of round", 100,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 1000));

  public static readonly FakeConVar<int> CV_INNOCENT_ARMOR = new(
    "css_ttt_rolearmor_innocent",
    "Amount of armor to give to innocents at start of round", 100,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 1000));

  public static readonly FakeConVar<string> CV_TRAITOR_WEAPONS = new(
    "css_ttt_roleweapons_traitor",
    "Weapons available to traitors at start of round",
    "weapon_knife,weapon_glock", ConVarFlags.FCVAR_NONE,
    new ItemValidator(allowMultiple: true));

  public static readonly FakeConVar<string> CV_DETECTIVE_WEAPONS = new(
    "css_ttt_roleweapons_detective",
    "Weapons available to detectives at start of round",
    "weapon_knife,weapon_taser,weapon_m4a1,weapon_revolver",
    ConVarFlags.FCVAR_NONE, new ItemValidator(allowMultiple: true));

  public static readonly FakeConVar<string> CV_INNOCENT_WEAPONS = new(
    "css_ttt_roleweapons_innocent",
    "Weapons available to innocents at start of round",
    "weapon_knife,weapon_glock", ConVarFlags.FCVAR_NONE,
    new ItemValidator(allowMultiple: true));

  public static readonly FakeConVar<int> CV_TIME_BETWEEN_ROUNDS = new(
    "css_ttt_time_between_rounds", "Time to wait between rounds in seconds", 5,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(1, 60));

  public void Dispose() { }

  public void Start() { }

  public void Start(BasePlugin? plugin) {
    ArgumentNullException.ThrowIfNull(plugin, nameof(plugin));
    plugin.RegisterFakeConVars(this);
  }

  public Task<TTTConfig?> Load() {
    return Task.FromResult<TTTConfig?>(new TTTConfig {
      RoleCfg = new TTTConfig.RoleConfig {
        TraitorHealth   = CV_TRAITOR_HEALTH.Value,
        DetectiveHealth = CV_DETECTIVE_HEALTH.Value,
        InnocentHealth  = CV_INNOCENT_HEALTH.Value,
        TraitorArmor    = CV_TRAITOR_ARMOR.Value,
        DetectiveArmor  = CV_DETECTIVE_ARMOR.Value,
        InnocentArmor   = CV_INNOCENT_ARMOR.Value,
        TraitorWeapons =
          CV_TRAITOR_WEAPONS.Value.Split(',')
           .Select(s => s.Trim())
           .Where(s => !string.IsNullOrEmpty(s))
           .ToArray(),
        DetectiveWeapons =
          CV_DETECTIVE_WEAPONS.Value.Split(',')
           .Select(s => s.Trim())
           .Where(s => !string.IsNullOrEmpty(s))
           .ToArray(),
        InnocentWeapons =
          CV_INNOCENT_WEAPONS.Value.Split(',')
           .Select(s => s.Trim())
           .Where(s => !string.IsNullOrEmpty(s))
           .ToArray()
      },
      RoundCfg = new CS2RoundConfig {
        CountDownDuration = TimeSpan.FromSeconds(CV_ROUND_COUNTDOWN.Value),
        MinimumPlayers    = CV_MINIMUM_PLAYERS.Value,
        TimeBetweenRounds = TimeSpan.FromSeconds(CV_TIME_BETWEEN_ROUNDS.Value)
      }
    });
  }

  public record CS2RoundConfig : TTTConfig.RoundConfig {
    public override TimeSpan RoundDuration(int players) {
      var baseDuration      = CV_ROUND_DURATION_BASE.Value;
      var perPlayerDuration = CV_ROUND_DURATION_PER_PLAYER.Value;
      var maxDuration       = CV_ROUND_DURATION_MAX.Value;

      var duration = baseDuration + (players - 1) * perPlayerDuration;
      return TimeSpan.FromSeconds(Math.Min(duration, maxDuration));
    }
  }
}