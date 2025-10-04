using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using TTT.API.Extensions;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Game.Roles;

namespace TTT.Shop.Items.Healthshot;

public static class HealthshotServiceCollection {
  public static void AddHealthshot(this IServiceCollection services) {
    services.AddModBehavior<HealthshotItem>();
  }
}

public class HealthshotItem(IServiceProvider provider) : BaseItem(provider) {
  private readonly HealthshotConfig config =
    provider.GetService<IStorage<HealthshotConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new HealthshotConfig();

  public override string Name => Locale[HealthshotMsgs.SHOP_ITEM_HEALTHSHOT];

  public override string Description
    => Locale[HealthshotMsgs.SHOP_ITEM_HEALTHSHOT_DESC];

  public override ShopItemConfig Config => config;

  public override void OnPurchase(IOnlinePlayer player) {
    Inventory.GiveWeapon(player, new BaseWeapon(config.Weapon));
  }

  public override PurchaseResult CanPurchase(IOnlinePlayer player) {
    return PurchaseResult.SUCCESS;
  }
}