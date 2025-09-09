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
  public void OnRoundStart(GameStateUpdateEvent ev) {
    // Only clear balances if the round is in progress
    // This is called only once, which means the round went from COUNTDOWN / WAITING -> IN_PROGRESS
    if (ev.NewState != State.IN_PROGRESS) return;
    shop.ClearBalances();
  }
}