using Microsoft.Extensions.DependencyInjection;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.Events.Player;
using TTT.Game.Roles;

namespace TTT.Game.Actions;

public class DamagedAction(IRoleAssigner roles, IPlayer victim,
  IPlayer attacker, string? weapon, int damage) : IAction {
  public DamagedAction(IRoleAssigner roles, PlayerDamagedEvent ev) : this(roles,
    ev.Player,
    ev.Attacker ?? throw new ArgumentNullException(nameof(ev.Attacker),
      "Attacker cannot be null"), ev.Weapon, ev.DmgDealt) { }


  public string? Weapon { get; } = weapon;
  public int Damage { get; } = damage;
  public IPlayer Player { get; } = attacker;
  public IPlayer? Other { get; } = victim;
  public IRole? PlayerRole { get; } = roles.GetRoles(attacker).FirstOrDefault();
  public IRole? OtherRole { get; } = roles.GetRoles(victim).FirstOrDefault();
  public string Id => "basegame.action.attack";
  public string Verb => "damaged";
  public string Details => $"for {Damage} damage with {Weapon}";

  public string Prefix
    => PlayerRole != null && OtherRole != null
      && PlayerRole is TraitorRole != OtherRole is TraitorRole ?
        "" :
        "[BAD] ";

  #region ConstructorAliases

  public DamagedAction(IServiceProvider provider, IPlayer victim,
    IPlayer attacker, string? weapon, int damage) : this(
    provider.GetRequiredService<IRoleAssigner>(), victim, attacker, weapon,
    damage) { }

  public DamagedAction(IServiceProvider provider, PlayerDamagedEvent ev) : this(
    provider.GetRequiredService<IRoleAssigner>(), ev) { }

  #endregion
}