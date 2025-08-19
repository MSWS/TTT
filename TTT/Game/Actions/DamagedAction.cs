using TTT.API.Game;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace TTT.Game.Actions;

public class DamagedAction(IPlayer victim, IPlayer attacker, string? weapon,
  int damage) : IAction {
  public DamagedAction(PlayerDamagedEvent ev) : this(ev.Player, ev.Attacker!,
  public DamagedAction(PlayerDamagedEvent ev)
    : this(
        ev.Player,
        ev.Attacker ?? throw new ArgumentNullException(nameof(ev.Attacker), "Attacker cannot be null"),
        ev.Weapon,
        ev.DmgDealt
      )
  { }

  public string? Weapon { get; } = weapon;
  public int Damage { get; } = damage;
  public IPlayer Player { get; } = attacker;
  public IPlayer? Other { get; } = victim;
  public string Id => "basegame.action.attack";
  public string Verb => "damaged";
  public string Details => $"for {Damage} damage with {Weapon}";
}