using TTT.Locale;

// ReSharper disable InconsistentNaming

namespace TTT.Game;

public static class GameMsgs {
  #region PREFIX

  public static IMsg PREFIX => MsgFactory.Create(nameof(PREFIX));

  #endregion

  #region ROLES

  public static IMsg ROLE_INNOCENT => MsgFactory.Create(nameof(ROLE_INNOCENT));
  public static IMsg ROLE_TRAITOR => MsgFactory.Create(nameof(ROLE_TRAITOR));

  public static IMsg ROLE_DETECTIVE
    => MsgFactory.Create(nameof(ROLE_DETECTIVE));

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

  #endregion
}