using ShopAPI;
using TTT.Locale;

namespace TTT.Shop;

public static class ShopMsgs {
  public static IMsg SHOP_INACTIVE => MsgFactory.Create(nameof(SHOP_INACTIVE));

  public static IMsg CREDITS_NAME => MsgFactory.Create(nameof(CREDITS_NAME));

  public static IMsg SHOP_CANNOT_PURCHASE
    => MsgFactory.Create(nameof(SHOP_CANNOT_PURCHASE));

  public static IMsg SHOP_PURCHASED(IShopItem item)
    => MsgFactory.Create(nameof(SHOP_PURCHASED), item.Name);

  public static IMsg SHOP_ITEM_NOT_FOUND(string query) {
    return MsgFactory.Create(nameof(SHOP_ITEM_NOT_FOUND), query);
  }

  public static IMsg CREDITS_GIVEN(int amo) {
    return MsgFactory.Create(nameof(CREDITS_GIVEN), amo > 0 ? "+" : "-",
      Math.Abs(amo));
  }

  public static IMsg CREDITS_GIVEN_REASON(int amo, string reason) {
    return MsgFactory.Create(nameof(CREDITS_GIVEN_REASON), amo > 0 ? "+" : "-",
      Math.Abs(amo), reason);
  }

  public static IMsg SHOP_INSUFFICIENT_BALANCE(IShopItem item, int bal) {
    return MsgFactory.Create(nameof(SHOP_INSUFFICIENT_BALANCE), item.Name,
      item.Config.Price, bal);
  }

  public static IMsg SHOP_CANNOT_PURCHASE_WITH_REASON(string reason) {
    return MsgFactory.Create(nameof(SHOP_CANNOT_PURCHASE_WITH_REASON), reason);
  }
}