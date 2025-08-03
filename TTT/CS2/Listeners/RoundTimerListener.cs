using CounterStrikeSharp.API;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Storage;
using TTT.Game;
using TTT.Game.Events.Game;

namespace TTT.CS2.Listeners;

public class RoundTimerListener(IServiceProvider provider) : IListener {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly GameConfig config = provider
   .GetRequiredService<IStorage<GameConfig>>()
   .Load()
   .GetAwaiter()
   .GetResult() ?? new GameConfig();

  public void Dispose() { bus.UnregisterListener(this); }

  [EventHandler(IgnoreCanceled = true)]
  public void OnRoundStart(GameStateUpdateEvent ev) {
    if (ev.NewState == State.COUNTDOWN) {
      RoundUtil.SetTimeRemaining((int)config.RoundCfg.CountDownDuration
       .TotalSeconds);
      Server.ExecuteCommand("mp_ignore_round_win_conditions 0");
      return;
    }

    if (ev.NewState != State.IN_PROGRESS) return;
    RoundUtil.SetTimeRemaining((int)config.RoundCfg
     .RoundDuration(ev.Game.Players.Count)
     .TotalSeconds);
    Server.ExecuteCommand("mp_ignore_round_win_conditions 1");
  }

  [EventHandler(IgnoreCanceled = true)]
  public void OnRoundEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;
    if (RoundUtil.GetTimeRemaining() <= 1) return;
    RoundUtil.SetTimeRemaining(0);
  }
}