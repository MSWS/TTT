using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using ShopAPI.Configs.Traitor;
using TTT.API.Extensions;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Game.Roles;

namespace TTT.CS2.Items.PoisonSmoke;

public static class PoisonSmokeServiceCollection {
  public static void AddPoisonSmokeServices(this IServiceCollection services) {
    services.AddModBehavior<PoisonSmokeItem>();
    services.AddModBehavior<PoisonSmokeListener>();
  }
}

public class PoisonSmokeItem(IServiceProvider provider)
  : RoleRestrictedItem<TraitorRole>(provider) {
  private PoisonSmokeConfig config
    => Provider.GetService<IStorage<PoisonSmokeConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new PoisonSmokeConfig();

  public override string Name => Locale[PoisonSmokeMsgs.SHOP_ITEM_POISON_SMOKE];

  public override string Description
    => Locale[PoisonSmokeMsgs.SHOP_ITEM_POISON_SMOKE_DESC];

  public override ShopItemConfig Config => config;

  public override void OnPurchase(IOnlinePlayer player) {
    Inventory.GiveWeapon(player, new BaseWeapon(config.Weapon));
  }

  public override PurchaseResult CanPurchase(IOnlinePlayer player) {
    return Shop.HasItem<PoisonSmokeItem>(player) ?
      PurchaseResult.ALREADY_OWNED :
      base.CanPurchase(player);
  }
}