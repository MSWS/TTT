using TTT.Locale;

namespace TTT.Game;

public static class GameMsgs {
  public static IMsg ROLE_INNOCENT => MsgFactory.Create(nameof(ROLE_INNOCENT));
  public static IMsg ROLE_TRAITOR => MsgFactory.Create(nameof(ROLE_TRAITOR));

  public static IMsg ROLE_DETECTIVE
    => MsgFactory.Create(nameof(ROLE_DETECTIVE));
}