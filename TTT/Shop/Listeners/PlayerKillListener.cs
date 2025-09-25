using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.Game.Events.Player;

namespace TTT.Shop.Listeners;

public class PlayerKillListener(IServiceProvider provider) : IListener {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();


  public void Dispose() { bus.UnregisterListener(this); }

  [UsedImplicitly]
  [EventHandler]
  public void OnKill(PlayerDeathEvent ev) { }
}