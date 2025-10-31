using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using ShopAPI.Configs.Traitor;
using TTT.API;
using TTT.API.Storage;

namespace TTT.CS2.Configs.ShopItems;

public class CS2ClusterGrenadeConfig : IStorage<ClusterGrenadeConfig>,
  IPluginModule {
  public static readonly FakeConVar<int> CV_PRICE = new(
    "css_ttt_shop_clustergrenade_price",
    "Price of the Cluster Grenade item (Traitor)", 100, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(0, 10000));

  public static readonly FakeConVar<int> CV_GRENADE_COUNT = new(
    "css_ttt_shop_clustergrenade_count",
    "Number of grenades released upon explosion", 8, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(1, 50));

  public static readonly FakeConVar<string> CV_WEAPON_ID = new(
    "css_ttt_shop_clustergrenade_weapon",
    "Weapon entity ID used for the Cluster Grenade", "weapon_hegrenade");

  public static readonly FakeConVar<float> CV_UP_FORCE = new(
    "css_ttt_shop_clustergrenade_up_force",
    "Upward force applied to cluster fragments", 200f, ConVarFlags.FCVAR_NONE,
    new RangeValidator<float>(0f, 1000f));

  public static readonly FakeConVar<float> CV_THROW_FORCE = new(
    "css_ttt_shop_clustergrenade_throw_force",
    "Forward throw force applied to cluster fragments", 250f,
    ConVarFlags.FCVAR_NONE, new RangeValidator<float>(0f, 1000f));

  public void Dispose() { }

  public void Start() { }

  public void Start(BasePlugin? plugin) {
    ArgumentNullException.ThrowIfNull(plugin, nameof(plugin));
    plugin.RegisterFakeConVars(this);
  }

  public Task<ClusterGrenadeConfig?> Load() {
    var cfg = new ClusterGrenadeConfig {
      Price        = CV_PRICE.Value,
      GrenadeCount = CV_GRENADE_COUNT.Value,
      UpForce      = CV_UP_FORCE.Value,
      ThrowForce   = CV_THROW_FORCE.Value
    };

    return Task.FromResult<ClusterGrenadeConfig?>(cfg);
  }
}