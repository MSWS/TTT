using TTT.Locale;

namespace TTT.CS2.Items.PoisonShots;

public class PoisonShotMsgs {
  public static IMsg SHOP_ITEM_POISON_SHOTS
    => MsgFactory.Create(nameof(SHOP_ITEM_POISON_SHOTS));

  public static IMsg SHOP_ITEM_POISON_SHOTS_DESC
    => MsgFactory.Create(nameof(SHOP_ITEM_POISON_SHOTS_DESC));

  public static IMsg SHOP_ITEM_POISON_OUT
    => MsgFactory.Create(nameof(SHOP_ITEM_POISON_OUT));
}