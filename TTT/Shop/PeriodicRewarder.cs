using System.Reactive.Concurrency;
using CounterStrikeSharp.API;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using TTT.API;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Storage;

namespace TTT.Shop;

public class PeriodicRewarder(IServiceProvider provider) : ITerrorModule {
  private readonly ShopConfig config = provider
   .GetService<IStorage<ShopConfig>>()
  ?.Load()
   .GetAwaiter()
   .GetResult() ?? new ShopConfig(provider);

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly IScheduler scheduler =
    provider.GetRequiredService<IScheduler>();

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  private IDisposable? timer;

  public void Dispose() { timer?.Dispose(); }

  public void Start() {
    timer = scheduler.SchedulePeriodic(config.CreditRewardInterval,
      issueRewards);
  }

  private void issueRewards() {
    if (games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    Server.NextWorldUpdate(() => {
      foreach (var player in finder.GetOnline().Where(p => p.IsAlive))
        shop.AddBalance(player, config.IntervalRewardAmount, "Alive");
    });
  }
}