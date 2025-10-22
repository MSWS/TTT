using TTT.Locale;

namespace Stats.lang;

public class StatsMsgs {
  public static IMsg API_ROUND_START(int roundId)
    => MsgFactory.Create(nameof(API_ROUND_START), roundId);

  public static IMsg API_ROUND_END(int roundId)
    => MsgFactory.Create(nameof(API_ROUND_END), roundId);
}