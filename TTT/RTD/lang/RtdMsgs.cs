using TTT.Locale;

namespace TTT.RTD.lang;

public class RtdMsgs {
  public static IMsg RTD_CANNOT_ROLL_YET
    => MsgFactory.Create(nameof(RTD_CANNOT_ROLL_YET));

  public static IMsg RTD_MUTED => MsgFactory.Create(nameof(RTD_MUTED));

  public static IMsg COMMAND_AUTORTD_ENABLED
    => MsgFactory.Create(nameof(COMMAND_AUTORTD_ENABLED));

  public static IMsg COMMAND_AUTORTD_DISABLED
    => MsgFactory.Create(nameof(COMMAND_AUTORTD_DISABLED));

  public static IMsg RTD_ALREADY_ROLLED(IRtdReward reward) {
    return MsgFactory.Create(nameof(RTD_ALREADY_ROLLED), reward.Name);
  }

  public static IMsg RTD_ROLLED(IRtdReward reward) {
    return MsgFactory.Create(nameof(RTD_ROLLED), reward.Name,
      reward.Description);
  }

  public static IMsg CREDITS_REWARD(int amo) {
    return MsgFactory.Create(nameof(CREDITS_REWARD), amo);
  }

  public static IMsg CREDITS_REWARD_DESC(int amo) {
    return MsgFactory.Create(nameof(CREDITS_REWARD_DESC), amo);
  }
}