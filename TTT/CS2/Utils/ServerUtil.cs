using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace TTT.CS2.Utils;

public static class ServerUtil {
  private static CCSGameRulesProxy? gamerulesProxy;

  public static CCSGameRules? GameRules {
    get {
      if (gamerulesProxy == null || !gamerulesProxy.IsValid) {
        gamerulesProxy = Utilities
         .FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules")
         .FirstOrDefault();
      }

      return gamerulesProxy?.GameRules;
    }
  }

  /// <summary>
  ///   Get the current CCSGameRules for the server
  /// </summary>
  /// <value></value>
  public static CCSGameRulesProxy? GameRulesProxy => gamerulesProxy;
}