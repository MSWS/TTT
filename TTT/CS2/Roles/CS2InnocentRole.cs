using CounterStrikeSharp.API.Modules.Utils;
using TTT.Game.Roles;

namespace TTT.CS2.Roles;

public class CS2InnocentRole(IServiceProvider provider)
  : InnocentRole(provider) {
  public override string Name => ChatColors.Green + base.Name;
}