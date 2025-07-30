using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TTT.API;
using TTT.API.Player;

namespace TTT.CS2;

public class CCPlayerConverter : IPluginModule,
  IPlayerConverter<CCSPlayerController> {
  private readonly Dictionary<string, CS2Player> playerCache = new();

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
    var gamePlayer = Utilities.GetPlayerFromSteamId(steamId);

    if (gamePlayer is { IsValid: true }) return gamePlayer;

    var bot = Utilities.GetPlayerFromIndex((int)steamId);
    return bot is { IsValid: true } ? bot : null;
  }

  public void Dispose() { playerCache.Clear(); }
  public string Name => "PlayerConverter";
  public string Version => GitVersionInformation.FullSemVer;

  public void Start() { }
}