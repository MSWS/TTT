using TTT.Locale;

namespace TTT.Shop.Items;

public class DeagleMsgs {
  public static IMsg SHOP_ITEM_DEAGLE
    => MsgFactory.Create(nameof(SHOP_ITEM_DEAGLE));

  public static IMsg SHOP_ITEM_DEAGLE_DESC
    => MsgFactory.Create(nameof(SHOP_ITEM_DEAGLE_DESC));

  public static IMsg SHOP_ITEM_DEAGLE_HIT_FF
    => MsgFactory.Create(nameof(SHOP_ITEM_DEAGLE_HIT_FF));

  public static IMsg SHOP_ITEM_DEAGLE_VICTIM
    => MsgFactory.Create(nameof(SHOP_ITEM_DEAGLE_VICTIM));
}