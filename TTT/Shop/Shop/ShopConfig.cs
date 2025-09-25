using Microsoft.Extensions.DependencyInjection;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.Events.Player;
using TTT.Game.Roles;

namespace TTT.Shop;

public record ShopConfig(IRoleAssigner assigner) {
  private static readonly Type[] roleConcerns = [
    typeof(TraitorRole), typeof(DetectiveRole), typeof(InnocentRole)
  ];

  public ShopConfig(IServiceProvider provider) : this(
    provider.GetRequiredService<IRoleAssigner>()) { }

  public int StartingInnocentCredits { get; init; } = 100;
  public int StartingTraitorCredits { get; init; } = 120;
  public int StartingDetectiveCredits { get; init; } = 150;
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
  public float CreditMultiplierForAssisting { get; init; } = 0.5f;
  public float CreditsMultiplierForNotAssisted { get; init; } = 1.5f;

  public virtual int CreditsForKill(IOnlinePlayer attacker,
    IOnlinePlayer victim) {
    var attackerRole = assigner.GetRoles(attacker)
     .FirstOrDefault(r => roleConcerns.Contains(r.GetType()));
    var victimRole = assigner.GetRoles(victim)
     .FirstOrDefault(r => roleConcerns.Contains(r.GetType()));

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

  public virtual int StartingCreditsForRole(IRole role)
    => role switch {
      TraitorRole   => StartingTraitorCredits,
      DetectiveRole => StartingDetectiveCredits,
      _             => StartingInnocentCredits
    };

  public virtual (int?, int?) CreditsFor(PlayerDeathEvent ev) {
    var victim   = ev.Victim;
    var killer   = ev.Killer;
    var assister = ev.Assister;

    if (victim == killer || killer is null) return (null, null);

    var killerCredits = CreditsForKill(killer, victim);
    var assisterCredits =
      assister == null ? 0 : CreditsForKill(assister, victim);

    assisterCredits = (int)(assisterCredits * CreditMultiplierForAssisting);
    if (assister == null)
      killerCredits = (int)(killerCredits * CreditsMultiplierForNotAssisted);

    return (killerCredits, assisterCredits);
  }
}