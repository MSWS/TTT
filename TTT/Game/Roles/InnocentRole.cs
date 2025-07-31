using System.Drawing;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Locale;

namespace TTT.Game.Roles;

public class InnocentRole(IMsgLocalizer? localizer = null) : IRole {
  public const string ID = "basegame.role.innocent";
  public string Id => ID;

  public string Name
    => localizer?[GameMsgs.ROLE_INNOCENT] ?? nameof(InnocentRole);

  public Color Color => Color.LimeGreen;

  public IOnlinePlayer? FindPlayerToAssign(ISet<IOnlinePlayer> players) {
    return players.FirstOrDefault(p => p.Roles.Count == 0);
  }
}