using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using ShopAPI.Configs;
using TTT.API;
using TTT.API.Storage;
using TTT.CS2.Validators;

namespace TTT.CS2.Configs.ShopItems;

public class CS2TaserConfig : IStorage<TaserConfig>, IPluginModule {
  public static readonly FakeConVar<int> CV_PRICE = new(
    "css_ttt_shop_taser_price", "Price of the Taser item", 110,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 10000));

  public static readonly FakeConVar<string> CV_WEAPON = new(
    "css_ttt_shop_taser_weapon", "Weapon entity name used for the Taser",
    "weapon_taser", ConVarFlags.FCVAR_NONE,
    new ItemValidator(allowMultiple: false));

  public void Dispose() { }

  public void Start() { }

  public void Start(BasePlugin? plugin) {
    ArgumentNullException.ThrowIfNull(plugin, nameof(plugin));
    plugin.RegisterFakeConVars(this);
  }

  public Task<TaserConfig?> Load() {
    var cfg = new TaserConfig {
      Price = CV_PRICE.Value, Weapon = CV_WEAPON.Value
    };

    return Task.FromResult<TaserConfig?>(cfg);
  }
}