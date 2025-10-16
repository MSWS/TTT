using TTT.Locale;

namespace TTT.CS2.Items.Compass;

public class CompassMsgs {
  public static IMsg SHOP_ITEM_COMPASS_PLAYER
    => MsgFactory.Create(nameof(SHOP_ITEM_COMPASS_PLAYER));

  public static IMsg SHOP_ITEM_COMPASS_PLAYER_DESC
    => MsgFactory.Create(nameof(SHOP_ITEM_COMPASS_PLAYER_DESC));

  public static IMsg SHOP_ITEM_COMPASS_BODY
    => MsgFactory.Create(nameof(SHOP_ITEM_COMPASS_BODY));

  public static IMsg SHOP_ITEM_COMPASS_BODY_DESC
    => MsgFactory.Create(nameof(SHOP_ITEM_COMPASS_BODY_DESC));
}