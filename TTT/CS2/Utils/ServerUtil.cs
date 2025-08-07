using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace TTT.CS2.Utils;

public static class ServerUtil {
  public static CCSGameRules? GameRules {
    get {
      if (GameRulesProxy == null || !GameRulesProxy.IsValid)
        GameRulesProxy = Utilities
         .FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules")
         .FirstOrDefault();

      return GameRulesProxy?.GameRules;
    }
  }

  /// <summary>
  ///   Get the current CCSGameRules for the server
  /// </summary>
  /// <value></value>
  public static CCSGameRulesProxy? GameRulesProxy { get; private set; }
}