using CounterStrikeSharp.API.Modules.Utils;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.Roles;
using TTT.Locale;

// ReSharper disable InconsistentNaming

namespace TTT.Game;

public static class GameMsgs {
  public static IMsg PREFIX => MsgFactory.Create(nameof(PREFIX));

  public static IMsg ROLE_INNOCENT => MsgFactory.Create(nameof(ROLE_INNOCENT));
  public static IMsg ROLE_TRAITOR => MsgFactory.Create(nameof(ROLE_TRAITOR));

  public static IMsg ROLE_DETECTIVE
    => MsgFactory.Create(nameof(ROLE_DETECTIVE));

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

  public static IMsg BODY_IDENTIFIED(IOnlinePlayer identifier, IPlayer ofPlayer,
    IRole role) {
    // TODO: Ideally we do this better
    var rolePrefix = role.GetType().IsAssignableTo(typeof(TraitorRole)) ?
      ChatColors.Red :
      role.GetType().IsAssignableTo(typeof(DetectiveRole)) ? ChatColors.Blue :
        ChatColors.Lime;

    return MsgFactory.Create(nameof(BODY_IDENTIFIED),
      identifier.Name ?? "Unknown Identifier",
      rolePrefix + (ofPlayer.Name ?? "Unknown Player"),
      role.Name ?? "Unknown Role");
  }

  public static IMsg GAME_LOGS_HEADER
    => MsgFactory.Create(nameof(GAME_LOGS_HEADER));

  #region COMMANDS

  public static IMsg CMD_TTT(string version) {
    return MsgFactory.Create(nameof(CMD_TTT), version);
  }

  #endregion

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