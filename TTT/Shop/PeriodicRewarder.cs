using System.Reactive.Concurrency;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using TTT.API;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.Extensions;

namespace TTT.Shop;

public class PeriodicRewarder(IServiceProvider provider) : ITerrorModule {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  private readonly Dictionary<string, List<Vector>> playerPositions = new();

  private readonly IScheduler scheduler =
    provider.GetRequiredService<IScheduler>();

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  private IDisposable? rewardTimer, updateTimer;

  private ShopConfig config
    => provider.GetService<IStorage<ShopConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new ShopConfig(provider);

  public void Dispose() {
    rewardTimer?.Dispose();
    updateTimer?.Dispose();
  }

  public void Start() {
    rewardTimer = scheduler.SchedulePeriodic(config.CreditRewardInterval,
      issueRewards);
    updateTimer = scheduler.SchedulePeriodic(config.PositionUpdateInterval,
      updatePositions);
  }

  private void issueRewards() {
    if (games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    Server.NextWorldUpdate(() => {
      var sortedPlayers = finder.GetOnline()
       .Where(p => p.IsAlive && playerPositions.ContainsKey(p.Id))
       .Select(p => (Player: p,
          Volume: getVolumeTraveled(
            playerPositions.GetValueOrDefault(p.Id, []))))
       .OrderByDescending(t => t.Volume)
       .ToList();

      var count = sortedPlayers.Count;
      for (var i = 0; i < count; i++) {
        var (player, _) = sortedPlayers[i];
        var position = count == 1 ? 1f : (float)(count - i - 1) / (count - 1);
        var rewardAmount = scaleRewardAmount(position, config.MinRewardAmount,
          config.MaxRewardAmount);
        shop.AddBalance(player, rewardAmount, "Exploration");
      }
    });
  }

  private void updatePositions() {
    if (games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    Server.NextWorldUpdate(() => {
      foreach (var player in finder.GetOnline().Where(p => p.IsAlive)) {
        var gamePlayer = converter.GetPlayer(player);
        var position   = gamePlayer?.Pawn.Value?.AbsOrigin;
        if (position is null) continue;
        position = position.Clone()!;

        var positions = playerPositions.GetValueOrDefault(player.Id, []);
        positions.Add(position);

        // Keep only the last N positions based on the interval
        var maxPositions = (int)(config.CreditRewardInterval.TotalSeconds
          / config.PositionUpdateInterval.TotalSeconds);
        while (positions.Count > maxPositions) positions.RemoveAt(0);

        playerPositions[player.Id] = positions;
      }
    });
  }

  private float getVolumeTraveled(List<Vector> positions) {
    if (positions.Count < 2) return 0f;
    var totalDistance = 0f;
    for (var i = 1; i < positions.Count; i++)
      totalDistance += positions[i].DistanceSquared(positions[i - 1]);


    return totalDistance;
  }

  /// <summary>
  ///   Scales a reward amount between min and max based on position (0-1).
  ///   0 = min, 1 = max.
  /// </summary>
  /// <param name="position"></param>
  /// <param name="min"></param>
  /// <param name="max"></param>
  /// <returns></returns>
  private int scaleRewardAmount(float position, int min, int max) {
    return (int)Math.Ceiling(min + (max - min) * position);
  }
}