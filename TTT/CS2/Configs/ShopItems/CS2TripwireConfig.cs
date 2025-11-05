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
    "css_ttt_shop_tripwire_price", "Price of the Tripwire item (Traitor)", 50,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 10000));

  public static readonly FakeConVar<int> CV_EXPLOSION_POWER = new(
    "css_ttt_shop_tripwire_explosion_power",
    "Explosion power of the Tripwire in damage units", 1000,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(1, 10000));

  public static readonly FakeConVar<float> CV_FALLOFF_DELAY = new(
    "css_ttt_shop_tripwire_falloff_delay",
    "Damage falloff, higher means faster falloff", 0.15f,
    ConVarFlags.FCVAR_NONE, new RangeValidator<float>(0f, 1f));

  public static readonly FakeConVar<float> CV_FF_MULTIPLIER = new(
    "css_ttt_shop_tripwire_friendlyfire_multiplier",
    "Damage multiplier applied to friendly fire from Tripwire", 0.5f,
    ConVarFlags.FCVAR_NONE, new RangeValidator<float>(0f, 1f));

  public static readonly FakeConVar<bool> CV_FF_TRIGGERS = new(
    "css_ttt_shop_tripwire_friendlyfire_triggers",
    "Whether Tripwires can be triggered by teammates", true);

  public void Dispose() { }

  public void Start() { }

  public void Start(BasePlugin? plugin) {
    ArgumentNullException.ThrowIfNull(plugin, nameof(plugin));
    plugin.RegisterFakeConVars(this);
  }

  public Task<TripwireConfig?> Load() {
    var cfg = new TripwireConfig {
      Price                  = CV_PRICE.Value,
      ExplosionPower         = CV_EXPLOSION_POWER.Value,
      FalloffDelay           = CV_FALLOFF_DELAY.Value,
      FriendlyFireMultiplier = CV_FF_MULTIPLIER.Value,
      FriendlyFireTriggers   = CV_FF_TRIGGERS.Value
    };

    return Task.FromResult<TripwireConfig?>(cfg);
  }
}