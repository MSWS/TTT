using Microsoft.Extensions.DependencyInjection;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.Events.Player;
using TTT.Game.Roles;

namespace TTT.Game.Actions;

public class DeathAction(IRoleAssigner roles, IPlayer victim, IPlayer? killer)
  : IAction {
  public DeathAction(IRoleAssigner roles, PlayerDeathEvent ev) : this(roles,
    ev.Player, ev.Killer) {
    if (!string.IsNullOrWhiteSpace(ev.Weapon)) Details = $"using {ev.Weapon}";
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
      $"{Prefix}{Other}{oRole} {Verb} {Player}{pRole} {Details}" :
      $"{Prefix}{Player}{pRole} {Verb} {Details}";
  }

  public string Prefix
    => PlayerRole != null && OtherRole != null
      && PlayerRole is TraitorRole != OtherRole is TraitorRole ?
        "" :
        "[BAD] ";

  #region ConstructorAliases

  public DeathAction(IServiceProvider provider, IPlayer victim, IPlayer? killer)
    : this(provider.GetRequiredService<IRoleAssigner>(), victim, killer) { }

  public DeathAction(IServiceProvider provider, PlayerDeathEvent ev) : this(
    provider.GetRequiredService<IRoleAssigner>(), ev) { }

  #endregion
}