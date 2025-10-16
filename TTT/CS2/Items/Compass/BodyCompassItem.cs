using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Extensions;
using TTT.API.Game;
using TTT.API.Player;
using TTT.CS2.API;
using TTT.CS2.Extensions;
using TTT.Game.Roles;

namespace TTT.CS2.Items.Compass;

public static class BodyCompassItemExtensions {
  public static void
    AddBodyCompassServices(this IServiceCollection collection) {
    collection.AddModBehavior<BodyCompassItem>();
  }
}

public class BodyCompassItem(IServiceProvider provider)
  : AbstractCompassItem<DetectiveRole>(provider) {
  private readonly IBodyTracker bodies =
    provider.GetRequiredService<IBodyTracker>();

  public override string Name => Locale[CompassMsgs.SHOP_ITEM_COMPASS_PLAYER];

  public override string Description
    => Locale[CompassMsgs.SHOP_ITEM_COMPASS_PLAYER_DESC];

  /// <summary>
  /// For innocents: point to nearest traitor.
  /// For traitors: point to nearest non-traitor (ally list in original code).
  /// Returns target world positions as vectors.
  /// </summary>
  protected override IList<Vector> GetTargets(IOnlinePlayer requester) {
    if (Games.ActiveGame is not { State: State.IN_PROGRESS or State.FINISHED })
      return Array.Empty<Vector>();

    List<Vector> vectors = [];

    foreach (var (apiBody, body) in bodies.Bodies) {
      if (apiBody.IsIdentified) continue;
      var origin = body.AbsOrigin.Clone();
      if (origin == null) continue;
      vectors.Add(origin);
    }

    return vectors;
  }

  override protected bool OwnsItem(IOnlinePlayer player) {
    return Shop.HasItem<BodyCompassItem>(player);
  }
}