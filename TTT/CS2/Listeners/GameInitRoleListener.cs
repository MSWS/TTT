using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.Game.Events.Game;

namespace TTT.CS2.Listeners;

public class GameInitRoleListener(IServiceProvider provider) : IListener {
  private IEventBus bus = provider.GetRequiredService<IEventBus>();
  public void Dispose() { bus.UnregisterListener(this); }

  [EventHandler]
  public void OnGameInit(GameInitEvent ev) {
    ev.Game.Roles.Insert(0, new SpectatorRole(provider));
  }
}