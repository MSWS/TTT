using Microsoft.Extensions.DependencyInjection;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.Events.Player;

namespace TTT.Game.Actions;

public class DeathAction(IRoleAssigner roles, IPlayer victim, IPlayer? killer)
  : IAction {
  public DeathAction(IRoleAssigner roles, PlayerDeathEvent ev) : this(roles,
    ev.Player, ev.Killer) {
    Details = $"using {ev.Weapon}";
  }

  public IPlayer Player { get; } = victim;
  public IPlayer? Other { get; } = killer;
  public IRole? PlayerRole { get; } = roles.GetRoles(victim).FirstOrDefault();

  public IRole? OtherRole { get; } = killer is not null ?
    roles.GetRoles(killer).FirstOrDefault() :
    null;

  public string Id { get; } = "basegame.action.death";
  public string Verb { get; } = killer is null ? "died" : "killed";

  public string Details { get; } = string.Empty;

  public string Format() {
    var pRole = PlayerRole != null ?
      $" [{PlayerRole.Name.First(char.IsAsciiLetter)}]" :
      "";
    var oRole = OtherRole != null ?
      $" [{OtherRole.Name.First(char.IsAsciiLetter)}]" :
      "";
    return Other is not null ?
      $"{Other}{oRole} {Verb} {Player}{pRole} {Details}" :
      $"{Player}{pRole} {Verb} {Details}";
  }

  #region ConstructorAliases

  public DeathAction(IServiceProvider provider, IPlayer victim, IPlayer? killer)
    : this(provider.GetRequiredService<IRoleAssigner>(), victim, killer) { }

  public DeathAction(IServiceProvider provider, PlayerDeathEvent ev) : this(
    provider.GetRequiredService<IRoleAssigner>(), ev) { }

  #endregion
}