using CounterStrikeSharp.API.Modules.Utils;
using TTT.Game.Roles;

namespace TTT.CS2.Roles;

public class CS2DetectiveRole(IServiceProvider provider)
  : DetectiveRole(provider) {
  public override string Name => ChatColors.Blue + base.Name;
}