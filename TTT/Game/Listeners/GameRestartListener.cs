using System.Reactive.Concurrency;
using System.Reactive.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Storage;
using TTT.Game.Events.Game;

namespace TTT.Game.Listeners;

public class GameRestartListener(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly TTTConfig config =
    provider.GetService<IStorage<TTTConfig>>()?.Load().GetAwaiter().GetResult()
    ?? new TTTConfig();

  private readonly IScheduler scheduler =
    provider.GetRequiredService<IScheduler>();

  [EventHandler]
  [UsedImplicitly]
  public void OnGameEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;
    Observable.Timer(config.RoundCfg.TimeBetweenRounds, scheduler)
     .Subscribe(_ => {
        if (Games.ActiveGame is { State: State.IN_PROGRESS or State.COUNTDOWN })
          return;
        Games.CreateGame()?.Start(config.RoundCfg.CountDownDuration);
      });
  }
}