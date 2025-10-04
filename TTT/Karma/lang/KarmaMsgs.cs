using TTT.Locale;

namespace TTT.Karma.lang;

public class KarmaMsgs {
  public static IMsg KARMA_COMMAND(int karma)
    => MsgFactory.Create(nameof(KARMA_COMMAND), karma);
}