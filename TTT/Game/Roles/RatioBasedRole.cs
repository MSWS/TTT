using System.Drawing;
using TTT.API.Player;

namespace TTT.Game.Roles;

public abstract class RatioBasedRole(IServiceProvider provider)
  : BaseRole(provider) {
  public override abstract string Id { get; }
  public override abstract string Name { get; }
  public override abstract Color Color { get; }

  abstract protected Func<int, int> TargetCount { get; }

  public override IOnlinePlayer?
    FindPlayerToAssign(ISet<IOnlinePlayer> players) {
    var currentCount     = players.Count(p => p.Roles.Any(r => r.Id == Id));
    var targetCountValue = TargetCount(players.Count);

    return currentCount >= targetCountValue ?
      null : // No need to assign this role
      players.FirstOrDefault(p => p.Roles.Count == 0);
  }
}