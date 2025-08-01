using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.Game.Events.Game;

namespace TTT.Shop;

public class RoundBalanceClearer(IServiceProvider provider) : IListener {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();
  private readonly IShop shop = provider.GetRequiredService<IShop>();

  public void Dispose() { bus.UnregisterListener(this); }

  [EventHandler(IgnoreCanceled = true, Priority = Priority.LOWER)]
  public void OnRoundEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.IN_PROGRESS) return;
    shop.ClearBalances();
  }
}