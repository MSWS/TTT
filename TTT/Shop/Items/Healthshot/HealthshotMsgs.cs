using TTT.Locale;

namespace TTT.Shop.Items.Healthshot;

public class HealthshotMsgs {
  public static IMsg SHOP_ITEM_HEALTHSHOT
    => MsgFactory.Create(nameof(SHOP_ITEM_HEALTHSHOT));

  public static IMsg SHOP_ITEM_HEALTHSHOT_DESC
    => MsgFactory.Create(nameof(SHOP_ITEM_HEALTHSHOT_DESC));
}