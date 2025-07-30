using System.Drawing;
using TTT.API;
using TTT.API.Player;
using TTT.API.Role;

namespace TTT.Game.Roles;

public class InnocentRole : IRole {
  public const string ID = "basegame.role.innocent";
  public string Id => ID;
  public string Name => "Innocent";
  public Color Color => Color.LimeGreen;

  public IOnlinePlayer? FindPlayerToAssign(ISet<IOnlinePlayer> players) {
    return players.FirstOrDefault(p => p.Roles.Count == 0);
  }
}