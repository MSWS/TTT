using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Storage;
using TTT.Game.Events.Game;

namespace TTT.Game.Listeners;

public class GameRestartListener(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly IGameManager games = provider
   .GetRequiredService<IGameManager>();

  private readonly IScheduler scheduler =
    provider.GetRequiredService<IScheduler>();

  private readonly GameConfig config = provider
   .GetRequiredService<IStorage<GameConfig>>()
   .Load()
   .GetAwaiter()
   .GetResult();

  public override string Name => nameof(GameRestartListener);

  [EventHandler]
  public void OnGameEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;
    Observable.Timer(config.RoundCfg.TimeBetweenRounds, scheduler)
     .Subscribe(_ => {
        if (games.IsGameActive()) return;
        games.CreateGame()?.Start(config.RoundCfg.CountDownDuration);
      });
  }
}