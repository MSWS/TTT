using System.Drawing;
using TTT.Api;
using TTT.Api.Player;

namespace TTT.Game.Roles;

public abstract class RatioBasedRole(float targetRatio) : IRole {
  public abstract string Id { get; }
  public abstract string Name { get; }
  public abstract Color Color { get; }

  public IOnlinePlayer? FindPlayerToAssign(ISet<IOnlinePlayer> players) {
    var currentCount = players.Count(p => p.Roles.Any(r => r.Id == Id));
    var ratio        = currentCount / (float)players.Count;
    return ratio >= targetRatio ? null : players.FirstOrDefault(p => p.Roles.Count == 0);
  }
}