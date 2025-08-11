using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TTT.API.Player;

namespace TTT.CS2;

public class CS2PlayerFinder(IPlayerConverter<CCSPlayerController> players)
  : IPlayerFinder {
  public IOnlinePlayer AddPlayer(IOnlinePlayer player) {
    throw new InvalidOperationException("Cannot call this within the game.");
  }

  public IPlayer RemovePlayer(IPlayer player) {
    throw new InvalidOperationException("Cannot call this within the game.");
  }

  public ISet<IOnlinePlayer> GetOnline() {
    return Utilities.GetPlayers()
     .Select(p => players.GetPlayer(p) as IOnlinePlayer)
     .ToHashSet()!;
  }
}