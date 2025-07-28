using System.Drawing;
using TTT.Api;
using TTT.Api.Player;

namespace TTT.Game.Roles;

public class TraitorRole(float targetRatio = 1f / 5f) : IRole {
  public const string ID = "core.role.traitor";
  public string Id => ID;
  public string Name => "Traitor";
  public Color Color => Color.Red;

  public IPlayer? FindPlayerToAssign(ISet<IOnlinePlayer> players) {
    var traitorCount = players.Count(p => p.Roles.Any(r => r.Id == ID));
    var ratio        = traitorCount / (float)players.Count;
    if (ratio >= targetRatio) return null;
    return players.First(p => p.Roles.Count == 0);
  }
}