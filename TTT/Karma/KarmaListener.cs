using System.Reactive.Concurrency;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.Events.Game;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;
using TTT.Game.Roles;

namespace TTT.Karma;

public class KarmaListener(IServiceProvider provider) : BaseListener(provider) {
  private static readonly int INNO_ON_TRAITOR = 2;
  private static readonly int TRAITOR_ON_DETECTIVE = 1;
  private static readonly int INNO_ON_INNO_VICTIM = -1;
  private static readonly int INNO_ON_INNO = -4;
  private static readonly int TRAITOR_ON_TRAITOR = -5;
  private static readonly int INNO_ON_DETECTIVE = -6;

  private readonly Dictionary<string, int> badKills = new();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  private readonly IKarmaService karma =
    provider.GetRequiredService<IKarmaService>();

  private readonly Dictionary<IPlayer, int> queuedKarmaUpdates = new();

  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  [EventHandler]
  [UsedImplicitly]
  public void OnRoundStart(GameStateUpdateEvent ev) { badKills.Clear(); }

  [EventHandler]
  [UsedImplicitly]
  public void OnKill(PlayerDeathEvent ev) {
    if (games.ActiveGame is not { State: State.IN_PROGRESS }) return;

    var victim = ev.Victim;
    var killer = ev.Killer;

    if (killer == null) return;

    var victimRole = roles.GetRoles(victim).First();
    var killerRole = roles.GetRoles(killer).First();

    var victimKarmaDelta = 0;
    var killerKarmaDelta = 0;

    var attackerKarmaMultiplier = 1;

    if (victimRole is TraitorRole == killerRole is TraitorRole) {
      badKills[killer.Id]     = badKills.GetValueOrDefault(killer.Id, 0) + 1;
      attackerKarmaMultiplier = badKills[killer.Id];
    }

    switch (victimRole) {
      case InnocentRole when killerRole is TraitorRole:
        return;
      case InnocentRole:
        victimKarmaDelta = INNO_ON_INNO_VICTIM;
        killerKarmaDelta = INNO_ON_INNO;
        break;
      case TraitorRole:
        killerKarmaDelta = killerRole is TraitorRole ?
          TRAITOR_ON_TRAITOR :
          INNO_ON_TRAITOR;
        break;
      case DetectiveRole:
        killerKarmaDelta = killerRole is TraitorRole ?
          TRAITOR_ON_DETECTIVE :
          INNO_ON_DETECTIVE;
        if (killerRole is DetectiveRole) victimKarmaDelta = INNO_ON_INNO_VICTIM;
        break;
    }

    killerKarmaDelta *= attackerKarmaMultiplier;

    queuedKarmaUpdates[killer] = queuedKarmaUpdates.GetValueOrDefault(killer, 0)
      + killerKarmaDelta;
    queuedKarmaUpdates[victim] = queuedKarmaUpdates.GetValueOrDefault(victim, 0)
      + victimKarmaDelta;
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnRoundEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;

    foreach (var (player, karmaDelta) in queuedKarmaUpdates)
      Task.Run(async () => {
        var newKarma = await karma.Load(player) + karmaDelta;
        await karma.Write(player, newKarma);
      });

    queuedKarmaUpdates.Clear();
  }
}