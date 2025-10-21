using TTT.API.Player;
using TTT.API.Role;
using TTT.Game;
using TTT.Locale;

namespace TTT.CS2.lang;

public static class CS2Msgs {
  public static IMsg ROLE_SPECTATOR
    => MsgFactory.Create(nameof(ROLE_SPECTATOR));

  public static IMsg DEAD_MUTE_REMINDER
    => MsgFactory.Create(nameof(DEAD_MUTE_REMINDER));

  public static IMsg TASER_SCANNED(IPlayer scannedPlayer, IRole role) {
    var rolePrefix = GameMsgs.GetRolePrefix(role);
    return MsgFactory.Create(nameof(TASER_SCANNED),
      rolePrefix + scannedPlayer.Name, role.Name);
  }

  public static IMsg AFK_WARNING(TimeSpan span) {
    return MsgFactory.Create(nameof(AFK_WARNING), span.TotalSeconds);
  }

  public static IMsg AFK_MOVED => MsgFactory.Create(nameof(AFK_MOVED));

  public static IMsg TRAITOR_CHAT_FORMAT(IOnlinePlayer player, string msg) {
    return MsgFactory.Create(nameof(TRAITOR_CHAT_FORMAT), player.Name, msg);
  }
}