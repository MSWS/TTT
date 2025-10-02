using TTT.Locale;

namespace TTT.Shop.Items.Detective.Stickers;

public class StickerMsgs {
  public static IMsg SHOP_ITEM_STICKERS
    => MsgFactory.Create(nameof(SHOP_ITEM_STICKERS));

  public static IMsg SHOP_ITEM_STICKERS_DESC
    => MsgFactory.Create(nameof(SHOP_ITEM_STICKERS_DESC));

  public static IMsg SHOP_ITEM_STICKERS_HIT
    => MsgFactory.Create(nameof(SHOP_ITEM_STICKERS_HIT));
}