using TTT.API.Game;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace TTT.Game.Actions;

public class DeathAction(IPlayer victim, IPlayer? killer) : IAction {
  public DeathAction(PlayerDeathEvent ev) : this(ev.Player, ev.Killer) {
    Details = $"using {ev.Weapon}";
  }

  public IPlayer Player { get; } = victim;
  public IPlayer? Other { get; } = killer;
  public string Id { get; } = "basegame.action.death";
  public string Verb { get; } = killer is null ? "died" : "killed";

  public string Details { get; } = string.Empty;

  public string Format() {
    return Other is not null ?
      $"{Other} {Verb} {Player} {Details}" :
      $"{Player} {Verb} {Details}";
  }
}