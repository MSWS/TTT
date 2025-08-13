using CounterStrikeSharp.API.Core;
using TTT.API.Events;
using TTT.API.Player;

namespace TTT.CS2.Events;

public class PropPickupEvent(IPlayer player, CBaseEntity ent)
  : PropEvent(player, ent), ICancelableEvent {
  public override string Id => "cs2.event.prop.pickup";
  public bool IsCanceled { get; set; }
}