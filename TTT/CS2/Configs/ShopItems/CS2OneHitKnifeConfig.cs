using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using ShopAPI.Configs.Traitor;
using TTT.API;
using TTT.API.Storage;

namespace TTT.CS2.Configs.ShopItems;

public class CS2OneHitKnifeConfig : IStorage<OneHitKnifeConfig>, IPluginModule {
  public static readonly FakeConVar<int> CV_PRICE = new(
    "css_ttt_shop_onehitknife_price",
    "Price of the One-Hit Knife item (Traitor)", 80, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(0, 10000));

  public static readonly FakeConVar<bool> CV_FRIENDLY_FIRE = new(
    "css_ttt_shop_onehitknife_friendly_fire",
    "Whether the One-Hit Knife can damage teammates", false,
    ConVarFlags.FCVAR_NONE);

  public void Dispose() { }

  public void Start() { }

  public void Start(BasePlugin? plugin) {
    ArgumentNullException.ThrowIfNull(plugin, nameof(plugin));
    plugin.RegisterFakeConVars(this);
  }

  public Task<OneHitKnifeConfig?> Load() {
    var cfg = new OneHitKnifeConfig {
      Price = CV_PRICE.Value, FriendlyFire = CV_FRIENDLY_FIRE.Value
    };

    return Task.FromResult<OneHitKnifeConfig?>(cfg);
  }
}