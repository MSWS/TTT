using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Extensions;
using TTT.API.Game;
using TTT.API.Player;
using TTT.CS2.Extensions;
using TTT.Game.Roles;

namespace TTT.CS2.Items.Compass;

public static class InnoCompassItemExtensions {
  public static void
    AddInnoCompassServices(this IServiceCollection collection) {
    collection.AddModBehavior<InnoCompassItem>();
  }
}

public class InnoCompassItem(IServiceProvider provider)
  : AbstractCompassItem<TraitorRole>(provider) {
  public override string Name => Locale[CompassMsgs.SHOP_ITEM_COMPASS_PLAYER];

  public override string Description
    => Locale[CompassMsgs.SHOP_ITEM_COMPASS_PLAYER_DESC];

  /// <summary>
  ///   For innocents: point to nearest traitor.
  ///   For traitors: point to nearest non-traitor (ally list in original code).
  ///   Returns target world positions as vectors.
  /// </summary>
  override protected IList<Vector> GetTargets(IOnlinePlayer requester) {
    if (Games.ActiveGame is not { State: State.IN_PROGRESS or State.FINISHED })
      return Array.Empty<Vector>();

    var all = Games.ActiveGame.Players.OfType<IOnlinePlayer>()
     .Where(p => p.IsAlive)
     .ToList();

    // Split by traitor role
    var traitors = all.Where(p => Roles.GetRoles(p).Any(r => r is TraitorRole))
     .ToList();
    var allies = all.Where(p => !Roles.GetRoles(p).Any(r => r is TraitorRole))
     .ToList();

    var enemies = Roles.GetRoles(requester).Any(r => r is TraitorRole) ?
      allies :
      traitors;

    // Convert to game controllers then to positions
    var vectors = new List<Vector>(enemies.Count);
    foreach (var enemy in enemies) {
      var controller = Converter.GetPlayer(enemy);
      var pos        = controller?.Pawn.Value?.AbsOrigin.Clone();
      if (pos != null) vectors.Add(pos);
    }

    return vectors;
  }

  override protected bool OwnsItem(IOnlinePlayer player) {
    return Shop.HasItem<InnoCompassItem>(player);
  }
}