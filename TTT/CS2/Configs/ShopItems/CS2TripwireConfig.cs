using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using ShopAPI.Configs.Traitor;
using TTT.API;
using TTT.API.Storage;

namespace TTT.CS2.Configs.ShopItems;

public class CS2TripwireConfig : IStorage<TripwireConfig>, IPluginModule {
  public static readonly FakeConVar<int> CV_PRICE = new(
    "css_ttt_shop_tripwire_price", "Price of the Tripwire item (Traitor)", 45,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 10000));

  public static readonly FakeConVar<int> CV_EXPLOSION_POWER = new(
    "css_ttt_shop_tripwire_explosion_power",
    "Explosion power of the Tripwire in damage units", 1000,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(1, 10000));

  public static readonly FakeConVar<float> CV_FALLOFF_DELAY = new(
    "css_ttt_shop_tripwire_falloff_delay",
    "Damage falloff of tripwire explosion, higher = quicker falloff", 0.015f,
    ConVarFlags.FCVAR_NONE, new RangeValidator<float>(0f, 1f));

  public static readonly FakeConVar<float> CV_FF_MULTIPLIER = new(
    "css_ttt_shop_tripwire_friendlyfire_multiplier",
    "Damage multiplier applied to friendly fire from Tripwire", 0.5f,
    ConVarFlags.FCVAR_NONE, new RangeValidator<float>(0f, 1f));

  public static readonly FakeConVar<bool> CV_FF_TRIGGERS = new(
    "css_ttt_shop_tripwire_friendlyfire_triggers",
    "Whether Tripwires can be triggered by teammates", true);

  public static readonly FakeConVar<float> CV_MAX_DISTANCE_SQUARED = new(
    "css_ttt_shop_tripwire_max_distance_squared",
    "Maximum placement distance squared for Tripwire", 50000f,
    ConVarFlags.FCVAR_NONE, new RangeValidator<float>(0f, 1000000f));

  public static readonly FakeConVar<float> CV_INITIATION_TIME = new(
    "css_ttt_shop_tripwire_initiation_time",
    "Seconds before Tripwire becomes active", 2f, ConVarFlags.FCVAR_NONE,
    new RangeValidator<float>(0f, 10f));

  public static readonly FakeConVar<float> CV_SIZE_SQUARED = new(
    "css_ttt_shop_tripwire_size_squared",
    "Size of tripwire for the purposes of bullet/defuse-detection", 10f,
    ConVarFlags.FCVAR_NONE, new RangeValidator<float>(1f, 100000f));

  public static readonly FakeConVar<int> CV_COLOR_R = new(
    "css_ttt_shop_tripwire_color_r", "Tripwire color red channel (0–255)", 255,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 255));

  public static readonly FakeConVar<int> CV_COLOR_G = new(
    "css_ttt_shop_tripwire_color_g", "Tripwire color green channel (0–255)", 0,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 255));

  public static readonly FakeConVar<int> CV_COLOR_B = new(
    "css_ttt_shop_tripwire_color_b", "Tripwire color blue channel (0–255)", 0,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 255));

  public static readonly FakeConVar<int> CV_COLOR_A = new(
    "css_ttt_shop_tripwire_color_a", "Tripwire color alpha (0–255)", 32,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 255));

  public static readonly FakeConVar<float> CV_THICKNESS = new(
    "css_ttt_shop_tripwire_thickness", "Visual thickness of the Tripwire beam",
    0.5f, ConVarFlags.FCVAR_NONE, new RangeValidator<float>(0.01f, 5f));

  public static readonly FakeConVar<float> CV_DEFUSE_TIME = new(
    "css_ttt_shop_tripwire_defuse_time",
    "Time required to fully defuse the Tripwire (in seconds)", 6f,
    ConVarFlags.FCVAR_NONE, new RangeValidator<float>(0f, 30f));

  public static readonly FakeConVar<float> CV_DEFUSE_RATE = new(
    "css_ttt_shop_tripwire_defuse_rate",
    "Rate at which Tripwire defuses are processed (in seconds)", 0.5f,
    ConVarFlags.FCVAR_NONE, new RangeValidator<float>(0.01f, 5f));

  public void Dispose() { }

  public void Start() { }

  public void Start(BasePlugin? plugin) {
    ArgumentNullException.ThrowIfNull(plugin, nameof(plugin));
    plugin.RegisterFakeConVars(this);
  }

  public Task<TripwireConfig?> Load() {
    var cfg = new TripwireConfig {
      Price = CV_PRICE.Value,
      ExplosionPower = CV_EXPLOSION_POWER.Value,
      FalloffDelay = CV_FALLOFF_DELAY.Value,
      FriendlyFireMultiplier = CV_FF_MULTIPLIER.Value,
      FriendlyFireTriggers = CV_FF_TRIGGERS.Value,
      MaxPlacementDistanceSquared = CV_MAX_DISTANCE_SQUARED.Value,
      TripwireInitiationTime = TimeSpan.FromSeconds(CV_INITIATION_TIME.Value),
      TripwireSizeSquared = CV_SIZE_SQUARED.Value,
      TripwireColor =
        Color.FromArgb(CV_COLOR_A.Value, CV_COLOR_R.Value, CV_COLOR_G.Value,
          CV_COLOR_B.Value),
      TripwireThickness = CV_THICKNESS.Value,
      DefuseTime        = TimeSpan.FromSeconds(CV_DEFUSE_TIME.Value),
      DefuseRate        = TimeSpan.FromSeconds(CV_DEFUSE_RATE.Value)
    };

    return Task.FromResult<TripwireConfig?>(cfg);
  }
}