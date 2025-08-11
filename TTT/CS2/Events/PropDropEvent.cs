using CounterStrikeSharp.API.Core;
using TTT.API.Player;

namespace TTT.CS2.Events;

public class PropDropEvent(IPlayer player, CBaseEntity ent)
  : PropEvent(player, ent) {
  public override string Id => "cs2.event.prop.drop";
}