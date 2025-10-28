using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;

namespace TTT.CS2.Actions;

public class TaserAction(IRoleAssigner roles, IPlayer victim,
  IPlayer identifier) : IAction {
  public IPlayer Player { get; } = identifier;
  public IPlayer? Other { get; } = victim;

  public IRole? PlayerRole { get; } =
    roles.GetRoles(identifier).FirstOrDefault();

  public IRole? OtherRole { get; } = roles.GetRoles(victim).FirstOrDefault();
  public string Id { get; } = "cs2.action.tased";
  public string Verb { get; } = "tased";
  public string Details { get; } = "";
}