using TTT.API.Player;
using TTT.API.Role;
using TTT.Game;
using TTT.Locale;

namespace TTT.CS2.Items.DNA;

public class DnaMsgs {
  public static IMsg SHOP_ITEM_DNA => MsgFactory.Create(nameof(SHOP_ITEM_DNA));

  public static IMsg SHOP_ITEM_DNA_DESC
    => MsgFactory.Create(nameof(SHOP_ITEM_DNA_DESC));

  public static IMsg SHOP_ITEM_DNA_SCANNED(IRole victimRole, IPlayer player,
    IPlayer killer) {
    return MsgFactory.Create(nameof(SHOP_ITEM_DNA_SCANNED),
      GameMsgs.GetRolePrefix(victimRole), player.Name, killer.Name);
  }

  public static IMsg SHOP_ITEM_DNA_SCANNED_OTHER(IRole victimRole,
    IPlayer player, string explanation) {
    return MsgFactory.Create(nameof(SHOP_ITEM_DNA_SCANNED_OTHER),
      GameMsgs.GetRolePrefix(victimRole), player.Name, explanation);
  }

  public static IMsg
    SHOP_ITEM_DNA_SCANNED_SUICIDE(IRole victimRole, IPlayer player) {
    return SHOP_ITEM_DNA_SCANNED_OTHER(victimRole, player,
      "they killed themselves");
  }

  public static IMsg SHOP_ITEM_DNA_EXPIRED(IRole victimRole, IPlayer player) {
    return MsgFactory.Create(nameof(SHOP_ITEM_DNA_EXPIRED),
      GameMsgs.GetRolePrefix(victimRole), player.Name);
  }
}