using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Entities;
using TTT.API.Player;

namespace TTT.CS2.Player;

public class CS2PermManager(IPlayerConverter<CCSPlayerController> converter)
  : IPermissionManager {
  public bool HasFlags(IPlayer player, params string[] flags) {
    if (flags.Length == 0) return true;
    var gamePlayer = converter.GetPlayer(player);
    if (gamePlayer == null) return false;
    return AdminManager.PlayerHasPermissions(gamePlayer, flags);
  }

  public bool InGroups(IPlayer player, params string[] groups) {
    if (groups.Length == 0) return true;
    var gamePlayer = converter.GetPlayer(player);

    var adminData = AdminManager.GetPlayerAdminData(gamePlayer);
    if (adminData == null) return false;
    return groups.All(g => adminData.Groups.Contains(g));
  }
}