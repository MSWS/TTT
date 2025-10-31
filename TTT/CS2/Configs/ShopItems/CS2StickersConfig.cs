using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using ShopAPI.Configs.Detective;
using TTT.API;
using TTT.API.Storage;

namespace TTT.CS2.Configs.ShopItems;

public class CS2StickersConfig : IStorage<StickersConfig>, IPluginModule {
  public static readonly FakeConVar<int> CV_PRICE = new(
    "css_ttt_shop_stickers_price", "Price of the Stickers item (Detective)", 35,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 10000));

  public void Dispose() { }

  public void Start() { }

  public void Start(BasePlugin? plugin) {
    ArgumentNullException.ThrowIfNull(plugin, nameof(plugin));
    plugin.RegisterFakeConVars(this);
  }

  public Task<StickersConfig?> Load() {
    var cfg = new StickersConfig { Price = CV_PRICE.Value };

    return Task.FromResult<StickersConfig?>(cfg);
  }
}