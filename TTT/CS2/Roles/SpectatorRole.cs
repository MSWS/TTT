using System.Drawing;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Locale;

namespace TTT.CS2.Roles;

public class SpectatorRole(IServiceProvider provider) : IRole {
  private readonly IPlayerConverter<CCSPlayerController> playerConverter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  public string Id => "basegame.role.spectator";

  public string Name
    => provider.GetRequiredService<IMsgLocalizer>()[CS2Msgs.ROLE_SPECTATOR];

  public Color Color => Color.Gray;

  public IOnlinePlayer? FindPlayerToAssign(ISet<IOnlinePlayer> players) {
    return players.FirstOrDefault(p
      => playerConverter.GetPlayer(p) is { Team: CsTeam.Spectator });
  }
}