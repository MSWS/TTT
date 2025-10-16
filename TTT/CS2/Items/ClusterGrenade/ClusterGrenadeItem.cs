using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using ShopAPI.Configs.Traitor;
using TTT.API.Extensions;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Game.Roles;

namespace TTT.CS2.Items.ClusterGrenade;

public static class ClusterGrenadeServiceCollection {
  public static void AddClusterGrenade(this IServiceCollection services) {
    services.AddModBehavior<ClusterGrenadeItem>();
    services.AddModBehavior<ClusterGrenadeListener>();
  }
}

public class ClusterGrenadeItem(IServiceProvider provider)
  : RoleRestrictedItem<TraitorRole>(provider) {
  private readonly ClusterGrenadeConfig config = provider
   .GetService<IStorage<ClusterGrenadeConfig>>()
  ?.Load()
   .GetAwaiter()
   .GetResult() ?? new ClusterGrenadeConfig();

  public override string Name
    => Locale[ClusterGrenadeMsgs.SHOP_ITEM_CLUSTER_GRENADE];

  public override string Description
    => Locale[ClusterGrenadeMsgs.SHOP_ITEM_CLUSTER_GRENADE_DESC];

  public override ShopItemConfig Config => config;

  public override void OnPurchase(IOnlinePlayer player) {
    Inventory.GiveWeapon(player, config);
  }
}