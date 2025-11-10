using CounterStrikeSharp.API.Modules.Utils;
using ShopAPI;
using TTT.API.Role;
using TTT.Locale;

namespace TTT.Shop;

public static class ShopMsgs {
  public static IMsg SHOP_PREFIX => MsgFactory.Create(nameof(SHOP_PREFIX));

  public static IMsg SHOP_INACTIVE => MsgFactory.Create(nameof(SHOP_INACTIVE));

  public static IMsg CREDITS_NAME => MsgFactory.Create(nameof(CREDITS_NAME));
  
  public static IMsg SHOP_EXPLORATION => MsgFactory.Create(nameof(SHOP_EXPLORATION));

  public static IMsg SHOP_CANNOT_PURCHASE
    => MsgFactory.Create(nameof(SHOP_CANNOT_PURCHASE));

  public static IMsg SHOP_PURCHASED(IShopItem item) {
    return MsgFactory.Create(nameof(SHOP_PURCHASED), item.Name);
  }

  public static IMsg SHOP_PURCHASED_DETAIL(IShopItem item) {
    return MsgFactory.Create(nameof(SHOP_PURCHASED_DETAIL), item.Description);
  }

  public static IMsg SHOP_ITEM_NOT_FOUND(string query) {
    return MsgFactory.Create(nameof(SHOP_ITEM_NOT_FOUND), query);
  }

  public static IMsg CREDITS_GIVEN(int amo) {
    return MsgFactory.Create(nameof(CREDITS_GIVEN), getCreditPrefix(amo),
      Math.Abs(amo));
  }

  public static IMsg CREDITS_GIVEN_REASON(int amo, string reason) {
    return MsgFactory.Create(nameof(CREDITS_GIVEN_REASON), getCreditPrefix(amo),
      Math.Abs(amo), reason);
  }

  private static string getCreditPrefix(int diff) {
    return diff > 0 ? ChatColors.Green + "+" : ChatColors.Red + "-";
  }

  public static IMsg SHOP_INSUFFICIENT_BALANCE(IShopItem item, int bal) {
    return MsgFactory.Create(nameof(SHOP_INSUFFICIENT_BALANCE), item.Name,
      item.Config.Price, bal);
  }

  public static IMsg SHOP_CANNOT_PURCHASE_WITH_REASON(string reason) {
    return MsgFactory.Create(nameof(SHOP_CANNOT_PURCHASE_WITH_REASON), reason);
  }

  public static IMsg COMMAND_BALANCE(int bal) {
    return MsgFactory.Create(nameof(COMMAND_BALANCE), bal);
  }

  public static IMsg SHOP_LIST_FOOTER(IRole role, int bal) {
    return MsgFactory.Create(nameof(SHOP_LIST_FOOTER), role.Name, bal);
  }
}