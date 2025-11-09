using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI.Configs.Detective;
using TTT.API.Extensions;
using TTT.API.Storage;
using TTT.CS2.Extensions;
using TTT.Game.Roles;

namespace TTT.CS2.Items.Station;

public static class HealthStationCollection {
  public static void
    AddHealthStationServices(this IServiceCollection collection) {
    collection.AddModBehavior<HealthStation>();
  }
}

public class HealthStation(IServiceProvider provider)
  : StationItem<DetectiveRole>(provider,
    provider.GetService<IStorage<HealthStationConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new HealthStationConfig()) {
  public override string Name => Locale[StationMsgs.SHOP_ITEM_STATION_HEALTH];

  public override string Description
    => Locale[StationMsgs.SHOP_ITEM_STATION_HEALTH_DESC];

  override protected void onInterval() {
    var players  = Utilities.GetPlayers();
    var toRemove = new List<CPhysicsPropMultiplayer>();

    // build per-player potential heal contributions and gather props to remove
    var perPlayerContrib = BuildPerPlayerContributions(players, toRemove);

    // apply the accumulated heals in a single pass per player
    applyAccumulatedHeals(perPlayerContrib);

    // remove invalid/expired props
    foreach (var prop in toRemove) Props.Remove(prop);
  }

  /// <summary>
  /// Scan all props and build a map: Player -> list of (StationInfo, potentialHeal).
  /// Also fills toRemove for invalid/expired props.
  /// </summary>
  private
    Dictionary<CCSPlayerController, List<(StationInfo info, int potential)>>
    BuildPerPlayerContributions(IReadOnlyList<CCSPlayerController> players,
      List<CPhysicsPropMultiplayer> toRemove) {
    var result =
      new Dictionary<CCSPlayerController, List<(StationInfo, int)>>();

    foreach (var (prop, info) in Props) {
      if (_Config.TotalHealthGiven != 0
        && Math.Abs(info.HealthGiven) > _Config.TotalHealthGiven) {
        toRemove.Add(prop);
        continue;
      }

      if (!prop.IsValid || prop.AbsOrigin == null) {
        toRemove.Add(prop);
        continue;
      }

      var propPos = prop.AbsOrigin;

      var playerDists = players
       .Select(p => (Player: p, Pos: p.Pawn.Value?.AbsOrigin))
       .Where(t => t is { Pos: not null, Player.Pawn.Value.Health: > 0 })
       .Select(t => (t.Player, Dist: t.Pos!.Distance(propPos)))
       .Where(t => t.Dist <= _Config.MaxRange)
       .ToList();

      foreach (var (player, dist) in playerDists) {
        var potentialHeal = ComputePotentialHeal(dist);
        if (potentialHeal <= 0) continue;

        if (!result.TryGetValue(player, out var list)) {
          list           = [];
          result[player] = list;
        }

        list.Add((info, potentialHeal));
      }
    }

    return result;
  }

  /// <summary>
  /// Compute potential heal from a station given the distance.
  /// Mirrors your original logic: ceil(HealthIncrements * healthScale).
  /// </summary>
  private int ComputePotentialHeal(double dist) {
    var healthScale = 1.0 - dist / _Config.MaxRange;
    return (int)Math.Ceiling(_Config.HealthIncrements * healthScale);
  }

  /// <summary>
  /// Apply heals to each player once per tick.
  /// Distributes the actual heal proportionally across contributing stations,
  /// updates station.Info.HealthGiven and emits a single pickup sound if needed.
  /// </summary>
  private void applyAccumulatedHeals(
    Dictionary<CCSPlayerController, List<(StationInfo info, int potential)>>
      perPlayerContrib) {
    foreach (var kv in perPlayerContrib) {
      var player   = kv.Key;
      var contribs = kv.Value;

      var maxHp     = player.Pawn.Value?.MaxHealth ?? 100;
      var currentHp = player.GetHealth();
      if (currentHp >= maxHp) continue;

      var totalPotential = contribs.Sum(c => c.potential);
      if (totalPotential <= 0) continue;

      var totalHealAvailable = Math.Min(totalPotential, maxHp - currentHp);
      if (totalHealAvailable <= 0) continue;

      var potentials = contribs.Select(c => c.potential).ToList();
      var allocations =
        allocateProportionalInteger(totalHealAvailable, potentials);

      // safety clamp: never allocate more than a station's potential
      for (int i = 0; i < allocations.Count; i++)
        if (allocations[i] > potentials[i])
          allocations[i] = potentials[i];

      // if clamping reduced the total, try to redistribute remaining amount
      var allocatedSum = allocations.Sum();
      if (allocatedSum < totalHealAvailable) {
        var remaining = totalHealAvailable - allocatedSum;
        for (var i = 0; i < allocations.Count && remaining > 0; i++) {
          var headroom = potentials[i] - allocations[i];
          if (headroom <= 0) continue;
          var give = Math.Min(headroom, remaining);
          allocations[i] += give;
          remaining      -= give;
        }

        allocatedSum = allocations.Sum();
      }

      // apply heal to player
      var actualAllocated = Math.Min(allocatedSum, maxHp - currentHp);
      if (actualAllocated <= 0) continue;

      var newHealth = Math.Min(currentHp + actualAllocated, maxHp);
      player.SetHealth(newHealth);

      // update station HealthGiven
      for (var i = 0; i < allocations.Count; i++) {
        var info                            = contribs[i].info;
        var allocated                       = allocations[i];
        if (allocated > 0) info.HealthGiven += allocated;
      }

      // emit a single pickup sound if any heal applied
      player.EmitSound("HealthShot.Pickup", null, 0.1f);
    }
  }

  /// <summary>
  /// Proportionally distribute an integer total across integer potentials.
  /// Uses floor of shares and assigns leftover to largest fractional remainders.
  /// Returns a list of allocations with same length as potentials.
  /// </summary>
  private List<int>
    allocateProportionalInteger(int total, List<int> potentials) {
    var allocations    = new List<int>(new int[potentials.Count]);
    var remainders     = new List<(int idx, double rem)>();
    var sumBase        = 0;
    var totalPotential = potentials.Sum();

    if (totalPotential <= 0) return allocations;

    for (var i = 0; i < potentials.Count; i++) {
      var potential = potentials[i];
      var share     = (double)potential / totalPotential * total;
      var baseAlloc = (int)Math.Floor(share);
      var rem       = share - baseAlloc;
      allocations[i] =  baseAlloc;
      sumBase        += baseAlloc;
      remainders.Add((i, rem));
    }

    var leftover = total - sumBase;
    if (leftover <= 0) return allocations;

    remainders = remainders.OrderByDescending(r => r.rem).ToList();
    var idx = 0;
    while (leftover > 0 && idx < remainders.Count) {
      allocations[remainders[idx].idx] += 1;
      leftover--;
      idx++;
    }

    return allocations;
  }
}