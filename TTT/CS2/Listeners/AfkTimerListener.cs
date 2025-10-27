using System.Drawing;
using System.Reactive.Concurrency;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.Extensions;
using TTT.CS2.lang;
using TTT.CS2.Utils;
using TTT.Game;
using TTT.Game.Events.Game;
using TTT.Game.Listeners;
using TTT.Game.Roles;

namespace TTT.CS2.Listeners;

public class AfkTimerListener(IServiceProvider provider)
  : BaseListener(provider) {
  private TTTConfig config
    => Provider.GetRequiredService<IStorage<TTTConfig>>()
     .Load()
     .GetAwaiter()
     .GetResult() ?? new TTTConfig();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private IDisposable? specTimer, specWarnTimer;

  public override void Dispose() {
    base.Dispose();

    specTimer?.Dispose();
    specWarnTimer?.Dispose();
  }

  [UsedImplicitly]
  [EventHandler(IgnoreCanceled = true)]
  public void OnRoundStart(GameStateUpdateEvent ev) {
    if (ev.NewState != State.IN_PROGRESS) {
      specTimer?.Dispose();
      specWarnTimer?.Dispose();
      return;
    }

    specWarnTimer?.Dispose();
    specWarnTimer = Scheduler.Schedule(config.RoundCfg.CheckAFKTimespan / 2, ()
      => {
      Server.NextWorldUpdate(() => {
        foreach (var player in getAfkPlayers()) {
          var apiPlayer = converter.GetPlayer(player);
          var timetill  = config.RoundCfg.CheckAFKTimespan / 2;
          Messenger.Message(apiPlayer, Locale[CS2Msgs.AFK_WARNING(timetill)]);
        }
      });
    });

    specTimer?.Dispose();
    specTimer = Scheduler.Schedule(config.RoundCfg.CheckAFKTimespan, () => {
      Server.NextWorldUpdate(() => {
        foreach (var player in getAfkPlayers()) {
          var apiPlayer = converter.GetPlayer(player);
#if !DEBUG
          player.ChangeTeam(CsTeam.Spectator);
#endif
          Messenger.Message(apiPlayer, Locale[CS2Msgs.AFK_MOVED]);
        }
      });
    });
  }

  private List<CCSPlayerController> getAfkPlayers() {
    return Utilities.GetPlayers()
     .Where(p => p.PlayerPawn.Value != null
        && p is { Team: CsTeam.CounterTerrorist or CsTeam.Terrorist }
        && p.GetHealth() >= 0 && !p.PlayerPawn.Value.HasMovedSinceSpawn)
     .ToList();
  }
}