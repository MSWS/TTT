using TTT.API.Game;
using TTT.API.Player;

namespace TTT.Game.Actions;

public class AttackAction(IPlayer attacker, IPlayer? target, string weapon,
  int damage) : IAction {
  public string Weapon { get; } = weapon;
  public int Damage { get; } = damage;
  public IPlayer Player { get; } = attacker;
  public IPlayer? Other { get; } = target;
  public string Id => "basegame.action.attack";
  public string Verb => "attacked";
  public string Details => $"for {Damage} damage with {Weapon}";
}