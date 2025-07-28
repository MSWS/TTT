using System.Drawing;
using TTT.Api;
using TTT.Api.Player;

namespace TTT.Game.Roles;

public abstract class RatioBasedRole(float targetRatio) : IRole {
  public abstract string Id { get; }
  public abstract string Name { get; }
  public abstract Color Color { get; }

  public IPlayer? FindPlayerToAssign(ISet<IOnlinePlayer> players) {
    var currentCount = players.Count(p => p.Roles.Any(r => r.Id == Id));
    var ratio        = currentCount / (float)players.Count;
    if (ratio >= targetRatio) return null;
    return players.First(p => p.Roles.Count == 0);
  }
}