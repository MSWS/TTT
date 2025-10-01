using TTT.API.Player;
using TTT.API.Role;
using TTT.Locale;

namespace TTT.CS2.Items.DNA;

public class DnaMsgs {
  public static IMsg SHOP_ITEM_DNA => MsgFactory.Create(nameof(SHOP_ITEM_DNA));

  public static IMsg SHOP_ITEM_DNA_DESC
    => MsgFactory.Create(nameof(SHOP_ITEM_DNA_DESC));

  public static IMsg SHOP_ITEM_DNA_SCANNED(IRole victimRole, IPlayer player,
    IPlayer killer)
    => MsgFactory.Create(nameof(SHOP_ITEM_DNA_SCANNED), victimRole.Name,
      player.Name, killer.Name);

  public static IMsg
    SHOP_ITEM_DNA_SCANNED_SUICIDE(IRole victimRole, IPlayer player)
    => MsgFactory.Create(nameof(SHOP_ITEM_DNA_SCANNED_SUICIDE));
}