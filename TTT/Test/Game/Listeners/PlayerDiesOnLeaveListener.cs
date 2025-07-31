using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace TTT.Test.Game.Listeners;

public class PlayerDiesOnLeaveListener(IServiceProvider provider) : IListener {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();
  public void Dispose() { bus.UnregisterListener(this); }

  [EventHandler(Priority = Priority.HIGH)]
  public void OnLeave(PlayerLeaveEvent ev) {
    if (ev.Player is not IOnlinePlayer online) return;
    online.IsAlive = false;
    var newEvent = new PlayerDeathEvent(ev.Player);
    bus.Dispatch(newEvent);
  }
}