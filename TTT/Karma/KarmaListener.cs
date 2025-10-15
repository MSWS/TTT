using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;
using TTT.API.Storage;
using TTT.Game.Events.Game;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;
using TTT.Game.Roles;

namespace TTT.Karma;

public class KarmaListener(IServiceProvider provider) : BaseListener(provider) {
  private readonly Dictionary<string, int> badKills = new();

  private readonly KarmaConfig config =
    provider.GetService<IStorage<KarmaConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new KarmaConfig();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  private readonly IKarmaService karma =
    provider.GetRequiredService<IKarmaService>();

  private readonly Dictionary<IPlayer, int> queuedKarmaUpdates = new();

  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  public bool GiveKarmaOnRoundEnd = true;

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
    if (victim.Id == killer.Id) return;

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
        victimKarmaDelta = config.INNO_ON_INNO_VICTIM;
        killerKarmaDelta = config.INNO_ON_INNO;
        break;
      case TraitorRole:
        killerKarmaDelta = killerRole is TraitorRole ?
          config.TRAITOR_ON_TRAITOR :
          config.INNO_ON_TRAITOR;
        break;
      case DetectiveRole:
        killerKarmaDelta = killerRole is TraitorRole ?
          config.TRAITOR_ON_DETECTIVE :
          config.INNO_ON_DETECTIVE;
        if (killerRole is DetectiveRole)
          victimKarmaDelta = config.INNO_ON_INNO_VICTIM;
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

    var winner = ev.Game.WinningRole;
    if (GiveKarmaOnRoundEnd)
      foreach (var player in ev.Game.Players)
        if (Roles.GetRoles(player).Any(r => r.GetType() == winner?.GetType()))
          queuedKarmaUpdates[player] =
            queuedKarmaUpdates.GetValueOrDefault(player, 0)
            + config.KarmaPerRoundWin;
        else
          queuedKarmaUpdates[player] =
            queuedKarmaUpdates.GetValueOrDefault(player, 0)
            + config.KarmaPerRound;

    foreach (var (player, karmaDelta) in queuedKarmaUpdates)
      Task.Run(async () => {
        var newKarma = await karma.Load(player) + karmaDelta;
        await karma.Write(player, newKarma);
      });

    queuedKarmaUpdates.Clear();
  }
}