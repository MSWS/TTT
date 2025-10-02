using TTT.Locale;

namespace TTT.Shop.Items.Traitor.Gloves;

public class GlovesMsgs {
  public static IMsg SHOP_ITEM_GLOVES
    => MsgFactory.Create(nameof(SHOP_ITEM_GLOVES));

  public static IMsg SHOP_ITEM_GLOVES_DESC
    => MsgFactory.Create(nameof(SHOP_ITEM_GLOVES_DESC));

  public static IMsg SHOP_ITEM_GLOVES_WORN_OUT
    => MsgFactory.Create(nameof(SHOP_ITEM_GLOVES_WORN_OUT));

  public static IMsg SHOP_ITEM_GLOVES_USED_BODY(int usesLeft, int maxUses) {
    return MsgFactory.Create(nameof(SHOP_ITEM_GLOVES_USED_BODY), usesLeft,
      maxUses);
  }

  public static IMsg SHOP_ITEM_GLOVES_USED_KILL(int usesLeft, int maxUses) {
    return MsgFactory.Create(nameof(SHOP_ITEM_GLOVES_USED_KILL), usesLeft,
      maxUses);
  }
}