using System.Drawing;
using TTT.Api;

namespace TTT.Core.Roles;

public class DetectiveRole : IRole {
  public const string ID = "core.role.detective";
  public string Id => ID;
  public string Name => "Detective";
  public Color Color => Color.DodgerBlue;

  public IPlayer? FindPlayerToAssign(ISet<IOnlinePlayer> players) {
    return players.FirstOrDefault(p => p.Roles.Count == 0);
  }
}