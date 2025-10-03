using ShopAPI;
using ShopAPI.Configs;
using TTT.API.Player;
using TTT.Game.Roles;

namespace TTT.Shop.Items.Traitor.BodyPaint;

public class BodyPaintItem(IServiceProvider provider)
  : RoleRestrictedItem<TraitorRole>(provider) {
  public override string Name => Locale[BodyPaintMsgs.SHOP_ITEM_BODY_PAINT];

  public override string Description
    => Locale[BodyPaintMsgs.SHOP_ITEM_BODY_PAINT_DESC];

  public override ShopItemConfig Config { get; }

  public override void OnPurchase(IOnlinePlayer player) { }
}