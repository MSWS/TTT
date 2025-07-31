using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Entities;
using TTT.API;
using TTT.API.Player;

namespace TTT.CS2;

public class CS2PermManager(IPlayerConverter<CCSPlayerController> converter)
  : IPermissionManager {
  public bool HasFlags(IPlayer player, params string[] flags) {
    var gamePlayer = converter.GetPlayer(player);
    return gamePlayer != null
      && AdminManager.PlayerHasPermissions(new SteamID(player.Id), flags);
  }

  public bool InGroups(IPlayer player, params string[] groups) {
    var gamePlayer = converter.GetPlayer(player);
    if (gamePlayer == null) return false;

    var adminData = AdminManager.GetPlayerAdminData(gamePlayer);
    if (adminData == null) return false;
    return groups.All(g => adminData.Groups.Contains(g));
  }
}