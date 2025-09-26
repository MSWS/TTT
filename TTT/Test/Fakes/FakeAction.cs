using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;

namespace TTT.Test.Fakes;

public class FakeAction(IPlayer player, IPlayer? other, IRole? playerRole,
  IRole? otherRole, string id, string verb, string details) : IAction {
  public IPlayer Player { get; } = player;
  public IPlayer? Other { get; } = other;
  public IRole? PlayerRole { get; } = playerRole;
  public IRole? OtherRole { get; } = otherRole;
  public string Id { get; } = id;
  public string Verb { get; } = verb;
  public string Details { get; } = details;
}