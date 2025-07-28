using CounterStrikeSharp.API.Core;
using GitVersion;
using TTT.Api;
using TTT.Api.Player;

namespace TTT.CS2;

public class CCPlayerConverter : IPluginModule,
  IPlayerConverter<CCSPlayerController> {
  private readonly Dictionary<string, CS2Player> playerCache = new();
  public void Dispose() { playerCache.Clear(); }
  public string Name => "PlayerConverter";
  public string Version => GitVersionInformation.FullSemVer;

  public void Start() { }

  public IPlayer GetPlayer(CCSPlayerController player) {
    if (playerCache.TryGetValue(player.SteamID.ToString(),
      out var cachedPlayer))
      return cachedPlayer;

    if (playerCache.TryGetValue(player.Index.ToString(),
      out var cachedPlayerByIndex))
      return cachedPlayerByIndex;

    if (player == null) {
      throw new ArgumentNullException(nameof(player), "Player cannot be null");
    }

    var newPlayer = new CS2Player(player);
    playerCache[newPlayer.Id] = newPlayer;
    return newPlayer;
  }
}