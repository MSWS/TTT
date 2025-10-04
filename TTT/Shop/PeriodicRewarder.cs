using System.Reactive.Concurrency;
using CounterStrikeSharp.API;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using TTT.API;
using TTT.API.Player;
using TTT.API.Storage;

namespace TTT.Shop;

public class PeriodicRewarder(IServiceProvider provider) : ITerrorModule {
  private readonly IScheduler scheduler =
    provider.GetRequiredService<IScheduler>();

  private IDisposable? timer;

  private readonly ShopConfig config = provider
   .GetService<IStorage<ShopConfig>>()
  ?.Load()
   .GetAwaiter()
   .GetResult() ?? new ShopConfig(provider);

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  public void Dispose() { timer?.Dispose(); }

  public void Start() {
    timer = scheduler.SchedulePeriodic(config.CreditRewardInterval,
      issueRewards);
  }

  private void issueRewards() {
    Server.NextWorldUpdate(() => { });
    foreach (var player in finder.GetOnline().Where(p => p.IsAlive))
      shop.AddBalance(player, config.IntervalRewardAmount, "Time Reward");
  }
}