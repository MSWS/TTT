using System.Drawing;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Player;
using TTT.API.Role;
using TTT.CS2.lang;
using TTT.Locale;

namespace TTT.CS2.Roles;

public class SpectatorRole(IServiceProvider provider) : IRole {
  private readonly IPlayerConverter<CCSPlayerController> playerConverter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  public string Id => "basegame.role.spectator";

  public string Name
    => provider.GetRequiredService<IMsgLocalizer>()[CS2Msgs.ROLE_SPECTATOR];

  public Color Color => Color.Gray;

  public IOnlinePlayer? FindPlayerToAssign(ISet<IOnlinePlayer> players) {
    return players.FirstOrDefault(p
      => roles.GetRoles(p).Count == 0
      && playerConverter.GetPlayer(p) is { Team: CsTeam.Spectator });
  }

  public void OnAssign(IOnlinePlayer player) {
    var csPlayer = playerConverter.GetPlayer(player);
    if (csPlayer is null) return;
    csPlayer.CommitSuicide(false, true);
    csPlayer.ChangeTeam(CsTeam.Spectator);
  }
}