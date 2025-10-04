using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace TTT.CS2.Utils;

public static class RoundUtil {
  // private static readonly
  //   MemoryFunctionVoid<nint, float, RoundEndReason, nint, nint>
  //   TerminateRoundFunc =
  //     new(GameData.GetSignature("CCSGameRules_TerminateRound"));

  private static IEnumerable<CCSTeam>? _teamManager;

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
    // TerminateRoundFunc.Invoke(gameRules.GameRules.Handle, 5f, reason, 0, 0);
    VirtualFunctions.TerminateRoundFunc.Invoke(gameRules.GameRules.Handle,
      reason, 5f, 0, 0);
  }

  public static void SetTeamScore(CsTeam team, int score) {
    _teamManager ??=
      Utilities.FindAllEntitiesByDesignerName<CCSTeam>("cs_team_manager");

    foreach (var entry in _teamManager) {
      if (entry.TeamNum != (byte)team) continue;
      entry.Score = score;
      Utilities.SetStateChanged(entry, "CTeam", "m_iScore");
      break;
    }
  }

  public static void AddTeamScore(CsTeam team, int score) {
    SetTeamScore(team, GetTeamScore(team) + score);
  }

  public static int GetTeamScore(CsTeam team) {
    _teamManager ??=
      Utilities.FindAllEntitiesByDesignerName<CCSTeam>("cs_team_manager");

    return (from entry in _teamManager
      where entry.TeamNum == (byte)team
      select entry.Score).FirstOrDefault();
  }
}