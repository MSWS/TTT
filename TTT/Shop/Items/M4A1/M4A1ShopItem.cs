using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using TTT.API;
using TTT.API.Extensions;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Game.Roles;

namespace TTT.Shop.Items.M4A1;

public static class M4A1ServiceCollection {
  public static void AddM4A1Services(this IServiceCollection collection) {
    collection.AddModBehavior<M4A1ShopItem>();
  }
}

public class M4A1ShopItem(IServiceProvider provider) : BaseItem(provider) {
  public override string Name => Locale[M4A1Msgs.SHOP_ITEM_M4A1];
  public override string Description => Locale[M4A1Msgs.SHOP_ITEM_M4A1_DESC];

  private readonly M4A1Config config =
    provider.GetService<IStorage<M4A1Config>>()?.Load().GetAwaiter().GetResult()
    ?? new M4A1Config();

  public override ShopItemConfig Config => config;

  public override void OnPurchase(IOnlinePlayer player) {
    Task.Run(async () => {
      foreach (var slot in config.ClearSlots)
        await Inventory.RemoveWeaponInSlot(player, slot);

      foreach (var weapon in config.Weapons)
        await Inventory.GiveWeapon(player, new BaseWeapon(weapon));
    });
  }

  public override PurchaseResult CanPurchase(IOnlinePlayer player) {
    return PurchaseResult.SUCCESS;
  }
}