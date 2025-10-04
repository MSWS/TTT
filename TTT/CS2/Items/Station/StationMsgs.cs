using TTT.Locale;

namespace TTT.CS2.Items.Station;

public class StationMsgs {
  public static IMsg SHOP_ITEM_STATION_HEALTH
    => MsgFactory.Create(nameof(SHOP_ITEM_STATION_HEALTH));

  public static IMsg SHOP_ITEM_STATION_HEALTH_DESC
    => MsgFactory.Create(nameof(SHOP_ITEM_STATION_HEALTH_DESC));

  public static IMsg SHOP_ITEM_STATION_HURT
    => MsgFactory.Create(nameof(SHOP_ITEM_STATION_HURT));

  public static IMsg SHOP_ITEM_STATION_HURT_DESC
    => MsgFactory.Create(nameof(SHOP_ITEM_STATION_HURT_DESC));
}