using System.Drawing;
using TTT.API.Player;
using TTT.API.Role;

namespace TTT.Game.Roles;

public abstract class RatioBasedRole(Func<int, int> targetCount) : IRole {
  public abstract string Id { get; }
  public abstract string Name { get; }
  public abstract Color Color { get; }

  public IOnlinePlayer? FindPlayerToAssign(ISet<IOnlinePlayer> players) {
    var currentCount     = players.Count(p => p.Roles.Any(r => r.Id == Id));
    var targetCountValue = targetCount(players.Count);

    return currentCount >= targetCountValue ?
      null : // No need to assign this role
      players.FirstOrDefault(p => p.Roles.Count == 0);
  }
}