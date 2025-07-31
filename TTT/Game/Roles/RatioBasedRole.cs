using System.Drawing;
using TTT.API.Player;
using TTT.API.Role;

namespace TTT.Game.Roles;

public abstract class RatioBasedRole(IServiceProvider provider,
  Func<int, int> targetCount) : BaseRole(provider) {
  public override abstract string Id { get; }
  public override abstract string Name { get; }
  public override abstract Color Color { get; }

  public override IOnlinePlayer?
    FindPlayerToAssign(ISet<IOnlinePlayer> players) {
    var currentCount     = players.Count(p => p.Roles.Any(r => r.Id == Id));
    var targetCountValue = targetCount(players.Count);

    return currentCount >= targetCountValue ?
      null : // No need to assign this role
      players.FirstOrDefault(p => p.Roles.Count == 0);
  }
}