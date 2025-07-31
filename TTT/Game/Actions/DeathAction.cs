using TTT.API.Game;
using TTT.API.Player;

namespace TTT.Game.Actions;

public class DeathAction(IPlayer victim, IPlayer? killer) : IAction {
  public IPlayer Player { get; } = victim;
  public IPlayer? Other { get; } = killer;
  public string Id { get; } = "basegame.action.death";
  public string Verb { get; } = killer is null ? "died" : "was killed by";

  public string Details { get; } = string.Empty;
}