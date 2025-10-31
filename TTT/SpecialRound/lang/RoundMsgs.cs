using SpecialRoundAPI;
using TTT.Locale;

namespace SpecialRound.lang;

public class RoundMsgs {
  public static IMsg SPECIAL_ROUND_SPEED
    => MsgFactory.Create(nameof(SPECIAL_ROUND_SPEED));

  public static IMsg SPECIAL_ROUND_BHOP
    => MsgFactory.Create(nameof(SPECIAL_ROUND_BHOP));

  public static IMsg SPECIAL_ROUND_VANILLA
    => MsgFactory.Create(nameof(SPECIAL_ROUND_VANILLA));

  public static IMsg VANILLA_ROUND_REMINDER
    => MsgFactory.Create(nameof(VANILLA_ROUND_REMINDER));

  public static IMsg SPECIAL_ROUND_STARTED(AbstractSpecialRound round) {
    return MsgFactory.Create(nameof(SPECIAL_ROUND_STARTED), round.Name);
  }
}