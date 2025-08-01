using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Storage;
using TTT.Game;
using TTT.Game.Events.Game;

namespace TTT.CS2.Listeners;

public class RoundStartTimerListener(IServiceProvider provider) : IListener {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly GameConfig config = provider
   .GetRequiredService<IStorage<GameConfig>>()
   .Load()
   .GetAwaiter()
   .GetResult();

  public void Dispose() { bus.UnregisterListener(this); }

  [EventHandler(IgnoreCanceled = true)]
  public void OnRoundStart(GameStateUpdateEvent ev) {
    if (ev.NewState == State.COUNTDOWN)
      RoundUtil.SetTimeRemaining((int)config.RoundCfg.CountDownDuration
       .TotalSeconds);

    if (ev.NewState != State.IN_PROGRESS) return;
    RoundUtil.SetTimeRemaining((int)config.RoundCfg
     .RoundDuration(ev.Game.Players.Count)
     .TotalSeconds);
  }
}