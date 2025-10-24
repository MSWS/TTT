using TTT.Locale;

namespace SpecialRound.lang;

public class RoundMsgs {
  public static IMsg SPECIAL_ROUND_STARTED(string name) {
    return MsgFactory.Create(nameof(SPECIAL_ROUND_STARTED), name);
  }

  public static IMsg SPECIAL_ROUND_SPEED
    => MsgFactory.Create(nameof(SPECIAL_ROUND_SPEED));
  
  public static IMsg SPECIAL_ROUND_BHOP
    => MsgFactory.Create(nameof(SPECIAL_ROUND_BHOP));
}