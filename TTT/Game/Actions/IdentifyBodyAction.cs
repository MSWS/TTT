using Microsoft.Extensions.DependencyInjection;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.Events.Body;

namespace TTT.Game.Actions;

public class IdentifyBodyAction(IRoleAssigner roles, BodyIdentifyEvent ev)
  : IAction {
  #region ConstructorAliases

  public IdentifyBodyAction(IServiceProvider provider, BodyIdentifyEvent ev) :
    this(provider.GetRequiredService<IRoleAssigner>(), ev) { }

  #endregion

  public IPlayer Player { get; } = ev.Identifier;
  public IPlayer? Other { get; } = ev.Body.OfPlayer;

  public IRole? PlayerRole { get; } =
    roles.GetRoles(ev.Identifier).FirstOrDefault();

  public IRole? OtherRole { get; } =
    roles.GetRoles(ev.Body.OfPlayer).FirstOrDefault();

  public string Id { get; } = "basegame.action.identify_body";
  public string Verb { get; } = "identified the body of";
  public string Details { get; } = "";
}