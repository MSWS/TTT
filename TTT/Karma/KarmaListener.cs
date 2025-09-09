using System.Diagnostics.Tracing;
using System.Reactive.Concurrency;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Role;
using TTT.Game.Events.Game;
using TTT.Game.Events.Player;
using TTT.Game.Roles;

namespace TTT.Karma;

public class KarmaListener(IServiceProvider provider) : IListener {
  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

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
  public Task OnKill(PlayerDeathEvent ev) {
    if (games.ActiveGame is not { State: State.IN_PROGRESS }) return Task.CompletedTask;

    var victim = ev.Victim;
    var killer = ev.Killer;

    if (killer == null) return Task.CompletedTask;

    var victimRole = roles.GetRoles(victim).First();
    var killerRole = roles.GetRoles(killer).First();

    var victimKarmaDelta = 0;
    var killerKarmaDelta = 0;

    if (victimRole is InnocentRole) {
      if (killerRole is TraitorRole) return Task.CompletedTask;
      victimKarmaDelta = INNO_ON_INNO_VICTIM;
      killerKarmaDelta = INNO_ON_INNO;

      innoOnInnoKills[killer.Id] =
        innoOnInnoKills.GetValueOrDefault(killer.Id, 0) + 1;

      killerKarmaDelta *= innoOnInnoKills[killer.Id];
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

    return Task.Run(async () => {
      var newKillerKarma = await karma.Load(killer) + killerKarmaDelta;
      var newVictimKarma = await karma.Load(victim) + victimKarmaDelta;

      await karma.Write(killer, newKillerKarma);
      await karma.Write(victim, newVictimKarma);
    });
  }
}