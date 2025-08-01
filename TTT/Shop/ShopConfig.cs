using TTT.API.Player;
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

  public int CreditsForKill(IOnlinePlayer attacker, IOnlinePlayer victim) {
    if (attacker.Roles.Any(r => r is TraitorRole)) {
      if (victim.Roles.Any(r => r is TraitorRole))
        return CreditsForTraitorVTraitorKill;
      if (victim.Roles.Any(r => r is InnocentRole))
        return CreditsForTraitorVInnoKill;
      if (victim.Roles.Any(r => r is DetectiveRole))
        return CreditsForTraitorVDetectiveKill;
    } else if (attacker.Roles.Any(r => r is InnocentRole)) {
      if (victim.Roles.Any(r => r is TraitorRole))
        return CreditsForInnoVTraitorKill;
      if (victim.Roles.Any(r => r is InnocentRole))
        return CreditsForInnoVInnoKill;
      if (victim.Roles.Any(r => r is DetectiveRole))
        return CreditsForInnoVDetectiveKill;
    }

    return CreditsForAnyKill;
  }
}