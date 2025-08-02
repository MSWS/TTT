using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.Roles;

namespace TTT.Shop;

public record ShopConfig {
  public int CreditsForRoundStart { get; init; } = 10;
  public int CreditsForInnoVInnoKill { get; init; } = -4;
  public int CreditsForInnoVTraitorKill { get; init; } = 8;
  public int CreditsForInnoVDetectiveKill { get; init; } = -6;
  public int CreditsForTraitorVTraitorKill { get; init; } = -5;
  public int CreditsForTraitorVInnoKill { get; init; } = 4;
  public int CreditsForTraitorVDetectiveKill { get; init; } = 6;
  public int CreditsForDetectiveVDetectiveKill { get; init; } = -8;
  public int CreditsForDetectiveVInnoKill { get; init; } = -6;
  public int CreditsForDetectiveVTraitorKill { get; init; } = 8;
  public int CreditsForAnyKill { get; init; } = 2;

  private readonly Dictionary<Type, Dictionary<Type, int>> creditsForKillCache =
    new() {
      {
        typeof(TraitorRole),
        new() {
          { typeof(TraitorRole), -5 },
          { typeof(InnocentRole), 4 },
          { typeof(DetectiveRole), 6 }
        }
      }, {
        typeof(InnocentRole),
        new() {
          { typeof(TraitorRole), 8 },
          { typeof(InnocentRole), -4 },
          { typeof(DetectiveRole), -6 }
        }
      }, {
        typeof(DetectiveRole),
        new() {
          { typeof(TraitorRole), 8 },
          { typeof(InnocentRole), -6 },
          { typeof(DetectiveRole), -8 }
        }
      }
    };


  public int CreditsForKill(IOnlinePlayer attacker, IOnlinePlayer victim) {
    var attackerRole = attacker.Roles.FirstOrDefault(r
      => creditsForKillCache.ContainsKey(r.GetType()));
    if (attackerRole == null) return CreditsForAnyKill;
    var victimRole = victim.Roles.FirstOrDefault(r
      => creditsForKillCache[attackerRole.GetType()].ContainsKey(r.GetType()));
    return victimRole == null ?
      CreditsForAnyKill :
      creditsForKillCache[attackerRole.GetType()][victimRole.GetType()];
  }
}