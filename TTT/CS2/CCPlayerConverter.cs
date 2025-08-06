using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Messages;
using TTT.API.Player;

namespace TTT.CS2;

public class CCPlayerConverter(IServiceProvider provider) : IPluginModule,
  IPlayerConverter<CCSPlayerController> {
  private readonly Lazy<IMessenger> msg =
    new(provider.GetRequiredService<IMessenger>);

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
    CCSPlayerController? result = null;
    msg.Value.Debug(
      $"Converting player {player.Id} ({player.Name}) to CCSPlayerController...");
    var gamePlayer = Utilities.GetPlayerFromSteamId(steamId);
    if (gamePlayer is { IsValid: true }) result = gamePlayer;

    var bot = Utilities.GetPlayerFromIndex((int)steamId);
    if (bot is { IsValid: true }) result = bot;
    return result;
  }

  public void Dispose() { playerCache.Clear(); }
  public string Name => "PlayerConverter";
  public string Version => GitVersionInformation.FullSemVer;

  public void Start() { }
}