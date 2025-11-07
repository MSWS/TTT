using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using TTT.API.Player;

namespace TTT.CS2.API.Items;

public record TripwireInstance(IOnlinePlayer owner, CEnvBeam Beam,
  CDynamicProp TripwireProp, Vector StartPos, Vector EndPos) {
  public override string ToString() {
    return
      $"TripwireInstance(Owner={owner}, StartPos={StartPos}, EndPos={EndPos})";
  }
}