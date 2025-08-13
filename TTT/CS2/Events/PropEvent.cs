using CounterStrikeSharp.API.Core;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace TTT.CS2.Events;

public abstract class PropEvent(IPlayer player, CBaseEntity ent)
  : PlayerEvent(player) {
  public CBaseEntity Prop { get; } = ent;
}