using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using ShopAPI.Configs.Traitor;
using TTT.API.Extensions;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Game.Roles;

namespace TTT.CS2.Items.TeleportDecoy;

public static class TeleportDecoyServiceCollection {
  public static void AddTeleportDecoyServices(
    this IServiceCollection collection) {
    collection.AddModBehavior<TeleportDecoyItem>();
    collection.AddModBehavior<TeleportDecoyListener>();
  }
}

public class TeleportDecoyItem(IServiceProvider provider)
  : RoleRestrictedItem<TraitorRole>(provider) {
  public override string Name
    => Locale[TeleportDecoyMsgs.SHOP_ITEM_TELEPORT_DECOY];

  public override string Description
    => Locale[TeleportDecoyMsgs.SHOP_ITEM_TELEPORT_DECOY_DESC];

  private TeleportDecoyConfig config
    => Provider.GetService<IStorage<TeleportDecoyConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new TeleportDecoyConfig();

  public override ShopItemConfig Config => config;

  public override void OnPurchase(IOnlinePlayer player) {
    Inventory.GiveWeapon(player, new BaseWeapon("weapon_decoy"));
  }
}