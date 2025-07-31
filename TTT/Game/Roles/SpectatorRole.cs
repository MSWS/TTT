using System.Drawing;
using TTT.API.Player;
using TTT.API.Role;

namespace TTT.Game.Roles;

public class SpectatorRole : IRole {
  public string Id => "basegame.role.spectator";
  public string Name => "Spectator";
  public Color Color => Color.Gray;

  public IOnlinePlayer? FindPlayerToAssign(ISet<IOnlinePlayer> players) {
    return null;
  }
}