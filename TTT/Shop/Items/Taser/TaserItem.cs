using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using TTT.API.Extensions;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Game.Roles;

namespace TTT.Shop.Items.Taser;

public static class TaserServiceCollection {
  public static void AddTaserItem(this IServiceCollection collection) {
    collection.AddModBehavior<TaserItem>();
  }
}

public class TaserItem(IServiceProvider provider) : BaseItem(provider) {
  private readonly TaserConfig config =
    provider.GetService<IStorage<TaserConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new TaserConfig();

  public override string Name => Locale[TaserMsgs.SHOP_ITEM_TASER];
  public override string Description => Locale[TaserMsgs.SHOP_ITEM_TASER_DESC];
  public override ShopItemConfig Config => config;

  public override void OnPurchase(IOnlinePlayer player) {
    // Remove in case they already have it, to allow refresh of recharging taser
    Inventory.RemoveWeapon(player, config.Weapon);
    Inventory.GiveWeapon(player, new BaseWeapon(config.Weapon));
  }

  public override PurchaseResult CanPurchase(IOnlinePlayer player) {
    return PurchaseResult.SUCCESS;
  }
}