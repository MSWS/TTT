using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using TTT.API.Extensions;
using TTT.API.Player;
using TTT.Game.Roles;

namespace TTT.CS2.Items.PoisonShots;

public static class PoisonShotServiceCollection {
  public static void AddPoisonShots(this IServiceCollection services) {
    services.AddModBehavior<PoisonShotsItem>();
    services.AddModBehavior<PoisonShotsListener>();
  }
}

public class PoisonShotsItem(IServiceProvider provider)
  : RoleRestrictedItem<TraitorRole>(provider) {
  public override string Name => Locale[PoisonShotMsgs.SHOP_ITEM_POISON_SHOTS];

  public override string Description
    => Locale[PoisonShotMsgs.SHOP_ITEM_POISON_SHOTS_DESC];

  public override ShopItemConfig Config { get; }

  public override void OnPurchase(IOnlinePlayer player) { }
}