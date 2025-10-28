using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using ShopAPI.Configs.Traitor;
using TTT.API;
using TTT.API.Storage;

namespace TTT.CS2.Configs.ShopItems;

public class CS2GlovesConfig : IStorage<GlovesConfig>, IPluginModule {
  public static readonly FakeConVar<int> CV_PRICE = new(
    "css_ttt_shop_gloves_price", "Price of the Gloves item (Traitor)", 40,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 10000));

  public static readonly FakeConVar<int> CV_MAX_USES = new(
    "css_ttt_shop_gloves_max_uses",
    "Maximum number of times the Gloves can be used before breaking", 5,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(1, 100));

  public void Dispose() { }

  public void Start() { }

  public void Start(BasePlugin? plugin) {
    ArgumentNullException.ThrowIfNull(plugin, nameof(plugin));
    plugin.RegisterFakeConVars(this);
  }

  public Task<GlovesConfig?> Load() {
    var cfg = new GlovesConfig {
      Price = CV_PRICE.Value, MaxUses = CV_MAX_USES.Value
    };

    return Task.FromResult<GlovesConfig?>(cfg);
  }
}