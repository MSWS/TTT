using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using ShopAPI.Configs;
using TTT.API;
using TTT.API.Storage;

namespace TTT.CS2.Configs;

public class CS2ShopConfig : IStorage<ShopConfig>, IPluginModule {
  public static readonly FakeConVar<int> CV_STARTING_INNOCENT_CREDITS = new(
    "css_ttt_shop_start_innocent", "Starting credits for Innocents", 60,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 10000));

  public static readonly FakeConVar<int> CV_STARTING_TRAITOR_CREDITS = new(
    "css_ttt_shop_start_traitor", "Starting credits for Traitors", 100,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 10000));

  public static readonly FakeConVar<int> CV_STARTING_DETECTIVE_CREDITS = new(
    "css_ttt_shop_start_detective", "Starting credits for Detectives", 120,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 10000));

  public static readonly FakeConVar<int> CV_INNO_V_INNO = new(
    "css_ttt_shop_inno_v_inno", "Credits change when Innocent kills Innocent",
    -4, ConVarFlags.FCVAR_NONE, new RangeValidator<int>(-10000, 10000));

  public static readonly FakeConVar<int> CV_INNO_V_TRAITOR = new(
    "css_ttt_shop_inno_v_traitor", "Credits change when Innocent kills Traitor",
    8, ConVarFlags.FCVAR_NONE, new RangeValidator<int>(-10000, 10000));

  public static readonly FakeConVar<int> CV_INNO_V_DETECTIVE = new(
    "css_ttt_shop_inno_v_detective",
    "Credits change when Innocent kills Detective", -6, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(-10000, 10000));

  public static readonly FakeConVar<int> CV_TRAITOR_V_TRAITOR = new(
    "css_ttt_shop_traitor_v_traitor",
    "Credits change when Traitor kills Traitor", -5, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(-10000, 10000));

  public static readonly FakeConVar<int> CV_TRAITOR_V_INNO = new(
    "css_ttt_shop_traitor_v_inno", "Credits change when Traitor kills Innocent",
    4, ConVarFlags.FCVAR_NONE, new RangeValidator<int>(-10000, 10000));

  public static readonly FakeConVar<int> CV_TRAITOR_V_DETECTIVE = new(
    "css_ttt_shop_traitor_v_detective",
    "Credits change when Traitor kills Detective", 6, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(-10000, 10000));

  public static readonly FakeConVar<int> CV_DETECTIVE_V_DETECTIVE = new(
    "css_ttt_shop_detective_v_detective",
    "Credits change when Detective kills Detective", -8, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(-10000, 10000));

  public static readonly FakeConVar<int> CV_DETECTIVE_V_INNO = new(
    "css_ttt_shop_detective_v_inno",
    "Credits change when Detective kills Innocent", -6, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(-10000, 10000));

  public static readonly FakeConVar<int> CV_DETECTIVE_V_TRAITOR = new(
    "css_ttt_shop_detective_v_traitor",
    "Credits change when Detective kills Traitor", 8, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(-10000, 10000));

  public static readonly FakeConVar<int> CV_ANY_KILL = new(
    "css_ttt_shop_any_kill",
    "Credits granted for any kill when roles are unknown", 2,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(-10000, 10000));

  public static readonly FakeConVar<float> CV_ASSIST_MULTIPLIER = new(
    "css_ttt_shop_assist_multiplier", "Multiplier applied to assister credits",
    0.5f, ConVarFlags.FCVAR_NONE, new RangeValidator<float>(0f, 10f));

  public static readonly FakeConVar<float> CV_SOLO_KILL_MULTIPLIER = new(
    "css_ttt_shop_solo_kill_multiplier",
    "Multiplier applied to killer credits when there is no assist", 1.5f,
    ConVarFlags.FCVAR_NONE, new RangeValidator<float>(0f, 10f));

  private readonly IServiceProvider _provider;

  public CS2ShopConfig(IServiceProvider provider) { _provider = provider; }

  public void Dispose() { }

  public void Start() { }

  public void Start(BasePlugin? plugin) {
    ArgumentNullException.ThrowIfNull(plugin, nameof(plugin));
    plugin.RegisterFakeConVars(this);
  }

  public Task<ShopConfig?> Load() {
    var cfg = new ShopConfig(_provider) {
      StartingInnocentCredits           = CV_STARTING_INNOCENT_CREDITS.Value,
      StartingTraitorCredits            = CV_STARTING_TRAITOR_CREDITS.Value,
      StartingDetectiveCredits          = CV_STARTING_DETECTIVE_CREDITS.Value,
      CreditsForInnoVInnoKill           = CV_INNO_V_INNO.Value,
      CreditsForInnoVTraitorKill        = CV_INNO_V_TRAITOR.Value,
      CreditsForInnoVDetectiveKill      = CV_INNO_V_DETECTIVE.Value,
      CreditsForTraitorVTraitorKill     = CV_TRAITOR_V_TRAITOR.Value,
      CreditsForTraitorVInnoKill        = CV_TRAITOR_V_INNO.Value,
      CreditsForTraitorVDetectiveKill   = CV_TRAITOR_V_DETECTIVE.Value,
      CreditsForDetectiveVDetectiveKill = CV_DETECTIVE_V_DETECTIVE.Value,
      CreditsForDetectiveVInnoKill      = CV_DETECTIVE_V_INNO.Value,
      CreditsForDetectiveVTraitorKill   = CV_DETECTIVE_V_TRAITOR.Value,
      CreditsForAnyKill                 = CV_ANY_KILL.Value,
      CreditMultiplierForAssisting      = CV_ASSIST_MULTIPLIER.Value,
      CreditsMultiplierForNotAssisted   = CV_SOLO_KILL_MULTIPLIER.Value
    };

    return Task.FromResult<ShopConfig?>(cfg);
  }
}