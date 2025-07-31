using System.Drawing;
using TTT.API.Player;
using TTT.API.Role;

namespace TTT.Game.Roles;

public abstract class BaseRole(IServiceProvider provider) : IRole {
  public abstract string Id { get; }
  public abstract string Name { get; }
  public abstract Color Color { get; }

  protected readonly IServiceProvider Provider = provider;

  public abstract IOnlinePlayer?
    FindPlayerToAssign(ISet<IOnlinePlayer> players);

  public virtual void OnAssign(IOnlinePlayer player) { }
}