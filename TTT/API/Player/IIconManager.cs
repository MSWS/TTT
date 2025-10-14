namespace TTT.API.Player;

/// <summary>
///   Assumes a maximum of 64 players.
///   Each bit in the bitmask represents whether a player is visible to the client.
///   Bit 0 is unused, bit 1 represents player 1, bit 2 represents player 2, and so on.
/// </summary>
public interface IIconManager {
  ulong GetVisiblePlayers(int client);
  void SetVisiblePlayers(int client, ulong playersBitmask);
  void RevealToAll(int client);
  void AddVisiblePlayer(int client, int player);
  void RemoveVisiblePlayer(int client, int player);
  void SetVisiblePlayers(IOnlinePlayer online, ulong playersBitmask);

  void ClearAllVisibility();
}