using CounterStrikeSharp.API.Modules.Utils;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.Roles;
using TTT.Locale;

namespace TTT.Game;

public static class GameMsgs {
  public static IMsg PREFIX => MsgFactory.Create(nameof(PREFIX));

  public static IMsg ROLE_INNOCENT => MsgFactory.Create(nameof(ROLE_INNOCENT));
  public static IMsg ROLE_TRAITOR => MsgFactory.Create(nameof(ROLE_TRAITOR));

  public static IMsg ROLE_DETECTIVE
    => MsgFactory.Create(nameof(ROLE_DETECTIVE));

  public static IMsg ROLE_REVEAL_TRAITORS_HEADER
    => MsgFactory.Create(nameof(ROLE_REVEAL_TRAITORS_HEADER));

  public static IMsg ROLE_REVEAL_TRAITORS_NONE
    => MsgFactory.Create(nameof(ROLE_REVEAL_TRAITORS_NONE));

  public static IMsg GAME_LOGS_HEADER
    => MsgFactory.Create(nameof(GAME_LOGS_HEADER));

  public static IMsg GAME_LOGS_FOOTER
    => MsgFactory.Create(nameof(GAME_LOGS_FOOTER));

  public static IMsg GAME_LOGS_NONE
    => MsgFactory.Create(nameof(GAME_LOGS_NONE));

  public static IMsg LOGS_VIEWED_INFO
    => MsgFactory.Create(nameof(LOGS_VIEWED_INFO));

  public static IMsg ROLE_REVEAL_DEATH(IRole killerRole) {
    return MsgFactory.Create(nameof(ROLE_REVEAL_DEATH),
      GetRolePrefix(killerRole) + killerRole.Name);
  }

  public static IMsg ROLE_ASSIGNED(IRole role) {
    return MsgFactory.Create(nameof(ROLE_ASSIGNED), role.Name);
  }

  public static IMsg GAME_STATE_STARTING(TimeSpan span) {
    return MsgFactory.Create(nameof(GAME_STATE_STARTING), span.TotalSeconds);
  }

  public static IMsg GAME_STATE_STARTED(int traitors, int nonTraitors) {
    return MsgFactory.Create(nameof(GAME_STATE_STARTED),
      traitors == 1 ? "is" : "are", traitors, nonTraitors);
  }

  public static IMsg GAME_STATE_ENDED_TEAM_WON(IRole team) {
    return MsgFactory.Create(nameof(GAME_STATE_ENDED_TEAM_WON), team.Name);
  }

  public static IMsg GAME_STATE_ENDED_OTHER(string reason) {
    return MsgFactory.Create(nameof(GAME_STATE_ENDED_OTHER), reason);
  }

  public static IMsg NOT_ENOUGH_PLAYERS(int minNeeded) {
    return MsgFactory.Create(nameof(NOT_ENOUGH_PLAYERS), minNeeded);
  }

  public static IMsg BODY_IDENTIFIED(IOnlinePlayer? identifier,
    IPlayer ofPlayer, IRole role) {
    // TODO: Ideally we do this better
    var rolePrefix = GetRolePrefix(role);
    return MsgFactory.Create(nameof(BODY_IDENTIFIED),
      identifier?.Name ?? "Someone", rolePrefix + ofPlayer.Name, role.Name);
  }

  public static char GetRolePrefix(IRole role) {
    return role.GetType().IsAssignableTo(typeof(TraitorRole)) ? ChatColors.Red :
      role.GetType().IsAssignableTo(typeof(DetectiveRole)) ?
        ChatColors.DarkBlue : ChatColors.Lime;
  }

  #region COMMANDS

  public static IMsg CMD_TTT(string version) {
    return MsgFactory.Create(nameof(CMD_TTT), version);
  }

  #endregion

  public static IMsg LOGS_VIEWED_ALIVE(IPlayer player) {
    return MsgFactory.Create(nameof(LOGS_VIEWED_ALIVE), player.Name);
  }

  #region GENERIC

  public static IMsg GENERIC_UNKNOWN(string command) {
    return MsgFactory.Create(nameof(GENERIC_UNKNOWN), command);
  }

  public static IMsg GENERIC_NO_PERMISSION
    => MsgFactory.Create(nameof(GENERIC_NO_PERMISSION));

  public static IMsg GENERIC_NO_PERMISSION_NODE(string node) {
    return MsgFactory.Create(nameof(GENERIC_NO_PERMISSION_NODE), node);
  }

  public static IMsg GENERIC_NO_PERMISSION_RANK(string rank) {
    return MsgFactory.Create(nameof(GENERIC_NO_PERMISSION_RANK), rank);
  }

  public static IMsg GENERIC_PLAYER_ONLY
    => MsgFactory.Create(nameof(GENERIC_PLAYER_ONLY));

  public static IMsg GENERIC_USAGE(string usage) {
    return MsgFactory.Create(nameof(GENERIC_USAGE), usage);
  }

  public static IMsg GENERIC_ERROR(string err) {
    return MsgFactory.Create(nameof(GENERIC_ERROR), err);
  }

  #endregion
}