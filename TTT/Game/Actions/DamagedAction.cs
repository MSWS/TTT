using TTT.API.Game;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace TTT.Game.Actions;

public class DamagedAction(IPlayer victim, IPlayer? attacker, string? weapon,
  int damage) : IAction {
  public string? Weapon { get; } = weapon;
  public int Damage { get; } = damage;
  public IPlayer Player { get; } = victim;
  public IPlayer? Other { get; } = attacker;
  public string Id => "basegame.action.attack";
  public string Verb => "damaged";
  public string Details => $"for {Damage} damage with {Weapon}";

  public DamagedAction(PlayerDamagedEvent ev) : this(ev.Player, ev.Attacker,
    ev.Weapon, ev.DmgDealt) { }
}