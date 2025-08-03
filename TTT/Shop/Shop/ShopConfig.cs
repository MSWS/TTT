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

  private static readonly Type[] roleConcerns = [
    typeof(TraitorRole), typeof(DetectiveRole), typeof(InnocentRole)
  ];

  public int CreditsForKill(IOnlinePlayer attacker, IOnlinePlayer victim) {
    var attackerRole =
      attacker.Roles.FirstOrDefault(r => roleConcerns.Contains(r.GetType()));
    var victimRole =
      victim.Roles.FirstOrDefault(r => roleConcerns.Contains(r.GetType()));

    if (attackerRole is null || victimRole is null) return CreditsForAnyKill;

    return attackerRole switch {
      TraitorRole when victimRole is TraitorRole =>
        CreditsForTraitorVTraitorKill,
      TraitorRole when victimRole is DetectiveRole =>
        CreditsForTraitorVDetectiveKill,
      TraitorRole when victimRole is InnocentRole => CreditsForTraitorVInnoKill,
      DetectiveRole when victimRole is DetectiveRole =>
        CreditsForDetectiveVDetectiveKill,
      DetectiveRole when victimRole is TraitorRole =>
        CreditsForDetectiveVTraitorKill,
      DetectiveRole when victimRole is InnocentRole =>
        CreditsForDetectiveVInnoKill,
      InnocentRole when victimRole is DetectiveRole =>
        CreditsForInnoVDetectiveKill,
      InnocentRole when victimRole is TraitorRole => CreditsForInnoVTraitorKill,
      InnocentRole when victimRole is InnocentRole => CreditsForInnoVInnoKill,
      _ => CreditsForAnyKill
    };
  }
}