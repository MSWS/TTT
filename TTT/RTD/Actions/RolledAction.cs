using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;

namespace TTT.RTD.Actions;

public class RolledAction(IRoleAssigner roles, IPlayer player, string roll)
  : IAction {
  public IPlayer Player { get; } = player;
  public IPlayer? Other { get; } = null;
  public IRole? PlayerRole { get; } = roles.GetRoles(player).FirstOrDefault();
  public IRole? OtherRole { get; } = null;
  public string Id { get; } = "rtd.action.rolled";
  public string Verb { get; } = "rolled";
  public string Details { get; } = roll;
}