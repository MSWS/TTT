using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;

namespace TTT.Game.Actions;

public class RoleAssignedAction(IPlayer player, IRole role) : IAction {
  public IPlayer Player { get; } = player;
  public IPlayer? Other => null;
  public IRole? PlayerRole { get; } = role;
  public IRole? OtherRole { get; } = null;
  public string Id => "basegame.action.roleassigned";
  public string Verb => "was assigned";

  public string Details { get; } =
    role.Name.Where(char.IsAsciiLetterOrDigit).ToString() ?? "?";
}