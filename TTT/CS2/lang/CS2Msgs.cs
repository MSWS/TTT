using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.lang;
using TTT.Locale;

namespace TTT.CS2.lang;

public static class CS2Msgs {
  public static IMsg ROLE_SPECTATOR
    => MsgFactory.Create(nameof(ROLE_SPECTATOR));

  public static IMsg TASER_SCANNED(IPlayer scannedPlayer, IRole role) {
    var rolePrefix = GameMsgs.GetRolePrefix(role);
    return MsgFactory.Create(nameof(TASER_SCANNED),
      rolePrefix + scannedPlayer.Name, role.Name);
  }
}