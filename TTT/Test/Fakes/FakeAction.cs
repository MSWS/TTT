using TTT.API;
using TTT.API.Game;
using TTT.API.Player;

namespace TTT.Test.Fakes;

public class FakeAction(IPlayer player, IPlayer? other, string id, string verb,
  string details) : IAction {
  public IPlayer Player { get; } = player;
  public IPlayer? Other { get; } = other;
  public string Id { get; } = id;
  public string Verb { get; } = verb;
  public string Details { get; } = details;
}