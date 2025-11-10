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

  public static IMsg SPECIAL_ROUND_SUPPRESSED
    => MsgFactory.Create(nameof(SPECIAL_ROUND_SUPPRESSED));

  public static IMsg SPECIAL_ROUND_SILENT
    => MsgFactory.Create(nameof(SPECIAL_ROUND_SILENT));

  public static IMsg SPECIAL_ROUND_PISTOL
    => MsgFactory.Create(nameof(SPECIAL_ROUND_PISTOL));

  public static IMsg SPECIAL_ROUND_RICH
    => MsgFactory.Create(nameof(SPECIAL_ROUND_RICH));

  public static IMsg SPECIAL_ROUND_LOWGRAV
    => MsgFactory.Create(nameof(SPECIAL_ROUND_LOWGRAV));

  public static IMsg SPECIAL_ROUND_STARTED(List<AbstractSpecialRound> round) {
    var roundNames = round.Count == 1 ?
      round[0].Name :
      string.Join(", ", round.Select(r => r.Name));
    return MsgFactory.Create(nameof(SPECIAL_ROUND_STARTED), roundNames);
  }
}