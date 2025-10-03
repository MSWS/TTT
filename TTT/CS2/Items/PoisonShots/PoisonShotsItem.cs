using ShopAPI;
using ShopAPI.Configs;
using TTT.API.Player;
using TTT.Game.Roles;

namespace TTT.CS2.Items.PoisonShots;

public class PoisonShotsItem(IServiceProvider provider)
  : RoleRestrictedItem<TraitorRole>(provider) {
  public override string Name => Locale[PoisonShotMsgs.SHOP_ITEM_POISON_SHOTS];

  public override string Description
    => Locale[PoisonShotMsgs.SHOP_ITEM_POISON_SHOTS_DESC];

  public override ShopItemConfig Config { get; }

  public override void OnPurchase(IOnlinePlayer player) { }
}