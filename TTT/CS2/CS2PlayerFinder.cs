using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TTT.API.Player;

namespace TTT.CS2;

public class CS2PlayerFinder(IPlayerConverter<CCSPlayerController> players)
  : IPlayerFinder {
  public void AddPlayer(IOnlinePlayer player) {
    throw new InvalidOperationException("Cannot call this within the game.");
  }

  public void RemovePlayer(IPlayer player) {
    throw new InvalidOperationException("Cannot call this within the game.");
  }

  public ISet<IOnlinePlayer> GetOnline() {
    return Utilities.GetPlayers()
     .Select(p => players.GetPlayer(p) as IOnlinePlayer)
     .ToHashSet()!;
  }
}