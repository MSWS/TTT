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
  private readonly TTTConfig config = provider
   .GetRequiredService<IStorage<TTTConfig>>()
   .Load()
   .GetAwaiter()
   .GetResult() ?? new TTTConfig();

  private readonly IScheduler scheduler =
    provider.GetRequiredService<IScheduler>();

  public override string Name => nameof(GameRestartListener);

  [EventHandler]
  [UsedImplicitly]
  public void OnGameEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;
    Observable.Timer(config.RoundCfg.TimeBetweenRounds, scheduler)
     .Subscribe(_ => {
        if (Games.IsGameActive()) return;
        Games.CreateGame()?.Start(config.RoundCfg.CountDownDuration);
      });
  }
}