using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Role;
using TTT.Game.Events.Game;
using TTT.Game.Events.Player;
using TTT.Game.Roles;

namespace TTT.Karma;

public class KarmaListener(IServiceProvider provider) : IListener {
  private static readonly int INNO_ON_TRAITOR = 2;
  private static readonly int TRAITOR_ON_DETECTIVE = 1;
  private static readonly int INNO_ON_INNO_VICTIM = -1;
  private static readonly int INNO_ON_INNO = -4;
  private static readonly int TRAITOR_ON_TRAITOR = -5;
  private static readonly int INNO_ON_DETECTIVE = -6;

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  private readonly Dictionary<string, int> badKills = new();

  private readonly IKarmaService karma =
    provider.GetRequiredService<IKarmaService>();

  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  public void Dispose() { }

  [EventHandler]
  [UsedImplicitly]
  public void OnRoundStart(GameStateUpdateEvent ev) { badKills.Clear(); }

  [EventHandler]
  [UsedImplicitly]
  public Task OnKill(PlayerDeathEvent ev) {
    if (games.ActiveGame is not { State: State.IN_PROGRESS })
      return Task.CompletedTask;

    var victim = ev.Victim;
    var killer = ev.Killer;

    if (killer == null) return Task.CompletedTask;

    var victimRole = roles.GetRoles(victim).First();
    var killerRole = roles.GetRoles(killer).First();

    var victimKarmaDelta = 0;
    var killerKarmaDelta = 0;

    var attackerKarmaMultiplier = 1;

    if (victimRole is TraitorRole == killerRole is TraitorRole) {
      badKills[killer.Id]     = badKills.GetValueOrDefault(killer.Id, 0) + 1;
      attackerKarmaMultiplier = badKills[killer.Id];
    }

    if (victimRole is InnocentRole) {
      if (killerRole is TraitorRole) return Task.CompletedTask;
      victimKarmaDelta = INNO_ON_INNO_VICTIM;
      killerKarmaDelta = INNO_ON_INNO;
    }

    if (victimRole is TraitorRole)
      killerKarmaDelta = killerRole is TraitorRole ?
        TRAITOR_ON_TRAITOR :
        INNO_ON_TRAITOR;

    if (victimRole is DetectiveRole) {
      killerKarmaDelta = killerRole is TraitorRole ?
        TRAITOR_ON_DETECTIVE :
        INNO_ON_DETECTIVE;
    }

    killerKarmaDelta *= attackerKarmaMultiplier;

    return Task.Run(async () => {
      var newKillerKarma = await karma.Load(killer) + killerKarmaDelta;
      var newVictimKarma = await karma.Load(victim) + victimKarmaDelta;

      await karma.Write(killer, newKillerKarma);
      await karma.Write(victim, newVictimKarma);
    });
  }
}