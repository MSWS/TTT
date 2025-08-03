using CounterStrikeSharp.API.Modules.Utils;
using TTT.Game.Roles;

namespace TTT.CS2.Roles;

public class CS2TraitorRole(IServiceProvider provider) : TraitorRole(provider) {
  public override string Name => ChatColors.Red + base.Name;
}