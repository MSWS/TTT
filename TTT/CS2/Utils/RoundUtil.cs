using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace TTT.CS2.Utils;

public static class RoundUtil {
  public static int GetTimeElapsed() {
    var gamerules = ServerUtil.GameRules;
    if (gamerules == null) return 0;
    var freezeTime = gamerules.FreezeTime;
    return (int)(Server.CurrentTime - gamerules.RoundStartTime - freezeTime);
  }

  public static int GetTimeRemaining() {
    var gamerules = ServerUtil.GameRules;
    if (gamerules == null) return 0;
    return gamerules.RoundTime - GetTimeElapsed();
  }

  public static void SetTimeRemaining(int seconds) {
    var gamerules = ServerUtil.GameRules;
    if (gamerules == null) return;
    gamerules.RoundTime = GetTimeElapsed() + seconds;
    var proxy = ServerUtil.GameRulesProxy;
    if (proxy == null) return;
    Utilities.SetStateChanged(proxy, "CCSGameRulesProxy", "m_pGameRules");
  }

  public static void AddTimeRemaining(int time) {
    var gamerules = ServerUtil.GameRules;
    if (gamerules == null) return;
    gamerules.RoundTime += time;

    var proxy = ServerUtil.GameRulesProxy;
    if (proxy == null) return;
    Utilities.SetStateChanged(proxy, "CCSGameRulesProxy", "m_pGameRules");
  }

  public static bool IsWarmup() {
    var rules = ServerUtil.GameRules;
    return rules == null || rules.WarmupPeriod;
  }

  public static void EndRound(RoundEndReason reason, float delay = 0) {
    ServerUtil.GameRulesProxy?.GameRules?.TerminateRound(delay, reason);
  }
}