using System.Diagnostics.Tracing;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Role;
using TTT.Game.Events.Game;
using TTT.Game.Events.Player;
using TTT.Game.Roles;

namespace TTT.Karma;

public class KarmaListener(IServiceProvider provider) : IListener {
  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  public void Dispose() { }

  private readonly Dictionary<string, int> innoOnInnoKills = new();

  private static readonly int INNO_ON_TRAITOR = 2;
  private static readonly int TRAITOR_ON_DETECTIVE = 1;
  private static readonly int INNO_ON_INNO_VICTIM = -1;
  private static readonly int INNO_ON_INNO = -4;
  private static readonly int TRAITOR_ON_TRAITOR = -5;
  private static readonly int INNO_ON_DETECTIVE = -6;

  private readonly IKarmaService karma =
    provider.GetRequiredService<IKarmaService>();

  [EventHandler]
  public void OnRoundStart(GameStateUpdateEvent ev) { innoOnInnoKills.Clear(); }

  [EventHandler]
  public void OnKill(PlayerDeathEvent ev) {
    var victim = ev.Victim;
    var killer = ev.Killer;

    if (killer == null) return;

    var victimRole = roles.GetRoles(victim).First();
    var killerRole = roles.GetRoles(killer).First();

    var victimKarmaDelta = 0;
    var killerKarmaDelta = 0;

    if (victimRole is InnocentRole) {
      if (killerRole is TraitorRole) return;
      victimKarmaDelta = INNO_ON_INNO_VICTIM;
      killerKarmaDelta = INNO_ON_INNO;

      innoOnInnoKills.TryGetValue(killer.Id, out var badRoundKills);

      killerKarmaDelta           *= badRoundKills;
      innoOnInnoKills[killer.Id] =  badRoundKills + 1;
    }

    if (victimRole is TraitorRole) {
      killerKarmaDelta = killerRole is TraitorRole ?
        TRAITOR_ON_TRAITOR :
        INNO_ON_TRAITOR;
    }

    if (victimRole is DetectiveRole) {
      killerKarmaDelta = killerRole is TraitorRole ?
        TRAITOR_ON_DETECTIVE :
        INNO_ON_DETECTIVE;
    }

    Task.Run(async () => {
      var newKillerKarma = await karma.Load(killer) + killerKarmaDelta;
      var newVictimKarma = await karma.Load(victim) + victimKarmaDelta;

      await karma.Write(killer, newKillerKarma);
      await karma.Write(victim, newVictimKarma);
    });
  }
}