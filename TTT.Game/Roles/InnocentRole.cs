using System.Drawing;
using TTT.Api;
using TTT.Api.Player;

namespace TTT.Core.Roles;

public class InnocentRole : IRole {
  public const string ID = "core.role.innocent";
  public string Id => ID;
  public string Name => "Innocent";
  public Color Color => Color.LimeGreen;

  public IPlayer? FindPlayerToAssign(ISet<IOnlinePlayer> players) {
    return players.FirstOrDefault(p => p.Roles.Count == 0);
  }
}