using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Storage;
using TTT.Game.Events.Player;

namespace TTT.Shop;

public class PlayerKillListener(IServiceProvider provider) : IListener {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly ShopConfig config =
    provider.GetService<IStorage<ShopConfig>>()?.Load().GetAwaiter().GetResult()
    ?? new ShopConfig(provider);

  // private readonly IRoleAssigner roles =
  //   provider.GetRequiredService<IRoleAssigner>();

  // private readonly IShop shop = provider.GetRequiredService<IShop>();

  public void Dispose() { bus.UnregisterListener(this); }

  [EventHandler]
  public void OnKill(PlayerDeathEvent ev) {
    var victim   = ev.Victim;
    var killer   = ev.Killer;
    var assister = ev.Assister;

    var (killerReward, assisterReward) = config.CreditsFor(ev);

    // TODO: We'll need to only give credits once the body is ID'd,
    // otherwise obvious meta-gaming can happen.

    // if (killer != null && killerReward.HasValue)
    //   shop.AddBalance(killer, killerReward.Value,
    //     "Killed " + ev.Victim?.Roles.First());
    // if (assister != null && assisterReward.HasValue)
    //   shop.AddBalance(assister, assisterReward.Value,
  }
}