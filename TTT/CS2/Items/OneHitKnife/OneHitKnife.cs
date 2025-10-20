using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using ShopAPI.Configs.Traitor;
using TTT.API.Extensions;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Game.Roles;

namespace TTT.CS2.Items.OneHitKnife;

public static class OneHitKnifeServiceCollection {
  public static void AddOneHitKnifeService(this IServiceCollection services) {
    services.AddModBehavior<OneHitKnife>();
    services.AddModBehavior<OneHitKnifeListener>();
  }
}

public class OneHitKnife(IServiceProvider provider)
  : RoleRestrictedItem<TraitorRole>(provider) {
  private OneHitKnifeConfig config
    => Provider.GetService<IStorage<OneHitKnifeConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new OneHitKnifeConfig();

  public override string Name
    => Locale[OneHitKnifeMsgs.SHOP_ITEM_ONE_HIT_KNIFE];

  public override string Description
    => Locale[OneHitKnifeMsgs.SHOP_ITEM_ONE_HIT_KNIFE_DESC];

  public override ShopItemConfig Config => config;

  public override void OnPurchase(IOnlinePlayer player) { }

  public override PurchaseResult CanPurchase(IOnlinePlayer player) {
    if (Shop.HasItem<OneHitKnife>(player)) return PurchaseResult.ALREADY_OWNED;
    return base.CanPurchase(player);
  }
}