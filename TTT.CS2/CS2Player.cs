using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TTT.Api;
using TTT.Api.Player;

namespace TTT.CS2;

/// <summary>
/// A CS2-specific implementation of <see cref="IOnlinePlayer"/>.
/// Human players will **always** be tracked by their Steam ID.
/// Non-human players (bots) will be tracked by their entity index.
///
/// Note that slot numbers are not guaranteed to be stable across server restarts.
/// </summary>
public class CS2Player : IOnlinePlayer {
  public string Id { get; }
  public string Name { get; }

  protected CS2Player(string id, string name) {
    Id   = id;
    Name = name;
  }

  public CS2Player(ulong steam) {
    Id = steam.ToString();
    var player = Utilities.GetPlayerFromSteamId(steam);
    Name = player?.PlayerName ?? Id;
  }

  public CS2Player(int index) {
    Id = index.ToString();
    var player = Utilities.GetPlayerFromIndex(index);
    Name = player?.PlayerName ?? Id;
  }

  public CS2Player(CCSPlayerController player) {
    Id   = GetKey(player);
    Name = player.PlayerName;
  }

  public static string GetKey(CCSPlayerController player) {
    if (player.IsBot || player.IsHLTV) return player.Index.ToString();
    return player.SteamID.ToString();
  }

  public ICollection<IRole> Roles { get; } = [];
}