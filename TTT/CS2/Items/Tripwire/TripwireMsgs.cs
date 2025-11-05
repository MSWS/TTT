using TTT.Locale;

namespace TTT.CS2.Items.Tripwire;

public class TripwireMsgs {
  public static IMsg SHOP_ITEM_TRIPWIRE
    => MsgFactory.Create(nameof(SHOP_ITEM_TRIPWIRE));

  public static IMsg SHOP_ITEM_TRIPWIRE_DESC
    => MsgFactory.Create(nameof(SHOP_ITEM_TRIPWIRE_DESC));

  public static IMsg SHOP_ITEM_TRIPWIRE_TOOFAR
    => MsgFactory.Create(nameof(SHOP_ITEM_TRIPWIRE_TOOFAR));
}