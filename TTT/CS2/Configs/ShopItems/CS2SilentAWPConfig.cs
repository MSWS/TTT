using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using ShopAPI.Configs.Traitor;
using TTT.API;
using TTT.API.Storage;

namespace TTT.CS2.Configs.ShopItems;

public class CS2SilentAWPConfig : IStorage<SilentAWPConfig>, IPluginModule {
  public static readonly FakeConVar<int> CV_PRICE = new(
    "css_ttt_shop_silentawp_price", "Price of the Silent AWP item (Traitor)",
    80, ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 10000));

  public static readonly FakeConVar<int> CV_WEAPON_INDEX = new(
    "css_ttt_shop_silentawp_index", "Weapon slot index for the Silent AWP", 9,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 64));

  public static readonly FakeConVar<string> CV_WEAPON_ID = new(
    "css_ttt_shop_silentawp_weapon", "Weapon entity ID for the Silent AWP",
    "weapon_awp");

  public static readonly FakeConVar<int> CV_RESERVE_AMMO = new(
    "css_ttt_shop_silentawp_reserve_ammo",
    "Reserve ammo count for the Silent AWP", 0, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(0, 100));

  public static readonly FakeConVar<int> CV_CURRENT_AMMO = new(
    "css_ttt_shop_silentawp_current_ammo",
    "Current ammo loaded in the Silent AWP", 1, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(0, 10));

  public void Dispose() { }

  public void Start() { }

  public void Start(BasePlugin? plugin) {
    ArgumentNullException.ThrowIfNull(plugin, nameof(plugin));
    plugin.RegisterFakeConVars(this);
  }

  public Task<SilentAWPConfig?> Load() {
    var cfg = new SilentAWPConfig {
      Price       = CV_PRICE.Value,
      WeaponIndex = CV_WEAPON_INDEX.Value,
      WeaponId    = CV_WEAPON_ID.Value,
      ReserveAmmo = CV_RESERVE_AMMO.Value,
      CurrentAmmo = CV_CURRENT_AMMO.Value
    };

    return Task.FromResult<SilentAWPConfig?>(cfg);
  }
}