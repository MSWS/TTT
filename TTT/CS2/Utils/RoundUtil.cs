using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace TTT.CS2.Utils;

public static class RoundUtil {
  private static readonly
    MemoryFunctionVoid<nint, float, RoundEndReason, nint, nint>
    TerminateRoundFunc =
      new(GameData.GetSignature("CCSGameRules_TerminateRound"));

  public static int GetTimeElapsed() {
    if (ServerUtil.GameRules == null) return 0;
    var freezeTime = ServerUtil.GameRules.FreezeTime;
    return (int)(Server.CurrentTime - ServerUtil.GameRules.RoundStartTime
      - freezeTime);
  }

  public static int GetTimeRemaining() {
    if (ServerUtil.GameRules == null) return 0;
    return ServerUtil.GameRules.RoundTime - GetTimeElapsed();
  }

  public static void SetTimeRemaining(int seconds) {
    if (ServerUtil.GameRules == null) return;
    ServerUtil.GameRules.RoundTime = GetTimeElapsed() + seconds;
    var proxy = ServerUtil.GameRulesProxy;
    if (proxy == null) return;
    Utilities.SetStateChanged(proxy, "CCSGameRulesProxy", "m_pGameRules");
  }

  public static void AddTimeRemaining(int time) {
    if (ServerUtil.GameRules == null) return;
    ServerUtil.GameRules.RoundTime += time;

    if (ServerUtil.GameRulesProxy == null) return;
    Utilities.SetStateChanged(ServerUtil.GameRulesProxy, "CCSGameRulesProxy",
      "m_pGameRules");
  }

  public static bool IsWarmup() {
    var rules = ServerUtil.GameRules;
    return rules == null || rules.WarmupPeriod;
  }

  public static void EndRound(RoundEndReason reason) {
    var gameRules = ServerUtil.GameRulesProxy;
    if (gameRules == null || gameRules.GameRules == null) return;
    // TODO: Figure out what these params do
    TerminateRoundFunc.Invoke(gameRules.GameRules.Handle, 5f, reason, 0, 0);
  }
}