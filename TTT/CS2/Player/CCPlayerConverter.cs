using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TTT.API;
using TTT.API.Player;

namespace TTT.CS2.Player;

public class CCPlayerConverter : IPluginModule,
  IPlayerConverter<CCSPlayerController> {
  private readonly Dictionary<string, CS2Player> playerCache = new();
  private readonly Dictionary<string, CCSPlayerController> reverseCache = new();

  public IPlayer GetPlayer(CCSPlayerController player) {
    if (playerCache.TryGetValue(player.SteamID.ToString(),
      out var cachedPlayer))
      return cachedPlayer;

    if (playerCache.TryGetValue(player.Index.ToString(),
      out var cachedPlayerByIndex))
      return cachedPlayerByIndex;

    if (player == null)
      throw new ArgumentNullException(nameof(player), "Player cannot be null");

    var newPlayer = new CS2Player(player);
    playerCache[newPlayer.Id] = newPlayer;
    return newPlayer;
  }

  public CCSPlayerController? GetPlayer(IPlayer player) {
    if (!ulong.TryParse(player.Id, out var steamId)) return null;
    if (reverseCache.TryGetValue(player.Id, out var cachedPlayer)) {
      if (cachedPlayer.IsValid) return cachedPlayer;

      reverseCache.Remove(player.Id);
    }

    CCSPlayerController? result = null;
    var                  gamePlayer = Utilities.GetPlayerFromSteamId(steamId);
    if (gamePlayer is { IsValid: true }) result = gamePlayer;

    var bot = Utilities.GetPlayerFromIndex((int)steamId);
    if (bot is { IsValid: true }) result = bot;
    if (result != null) reverseCache[player.Id] = result;
    return result;
  }

  public void Dispose() { playerCache.Clear(); }

  public void Start() { }
}