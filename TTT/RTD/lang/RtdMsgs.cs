using TTT.Locale;

namespace TTT.RTD.lang;

public class RtdMsgs {
  public static IMsg RTD_ALREADY_ROLLED(IRtdReward reward)
    => MsgFactory.Create(nameof(RTD_ALREADY_ROLLED), reward.Name);

  public static IMsg RTD_CANNOT_ROLL_YET
    => MsgFactory.Create(nameof(RTD_CANNOT_ROLL_YET));

  public static IMsg RTD_ROLLED(IRtdReward reward)
    => MsgFactory.Create(nameof(RTD_ROLLED), reward.Name, reward.Description);
}