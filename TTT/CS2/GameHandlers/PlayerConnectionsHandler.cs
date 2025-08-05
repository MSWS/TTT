using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using TTT.API;
using TTT.API.Events;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace TTT.CS2.GameHandlers;

public class PlayerConnectionsHandler(IEventBus bus,
  IPlayerConverter<CCSPlayerController> converter) : IPluginModule {
  public string Name => nameof(PlayerConnectionsHandler);
  public string Version => GitVersionInformation.FullSemVer;

  public void Start() { }

  public void Start(BasePlugin? plugin, bool hotReload) {
    Console.WriteLine(
      $"PlayerConnectionsHandler started, hotReload: {hotReload}");
    plugin
    ?.RegisterListener<CounterStrikeSharp.API.Core.Listeners.OnClientConnect>(
        connectToServer);
    plugin
    ?.RegisterListener<
        CounterStrikeSharp.API.Core.Listeners.OnClientDisconnect>(
        disconnectFromServer);

    if (!hotReload) return;

    // Delay dispatching PlayerJoinEvent since our listeners may
    // not yet be registered.
    Server.NextWorldUpdate(() => {
      foreach (var player in Utilities.GetPlayers()) {
        var gamePlayer = converter.GetPlayer(player);
        var ev         = new PlayerJoinEvent(gamePlayer);
        Console.WriteLine(
          $"Dispatching PlayerJoinEvent for player {gamePlayer.Id} ({gamePlayer.Name})");
        bus.Dispatch(ev);
      }
    });
  }

  private void disconnectFromServer(int playerSlot) {
    var player = Utilities.GetPlayerFromSlot(playerSlot);
    Console.WriteLine($"Player {playerSlot} disconnected from server.");
    if (player == null || !player.IsValid) {
      Console.WriteLine($"Player {playerSlot} does not exist.");
      return;
    }

    var gamePlayer = converter.GetPlayer(player);
    bus.Dispatch(new PlayerLeaveEvent(gamePlayer));
  }

  private void connectToServer(int playerSlot, string name, string ipAddress) {
    var player = Utilities.GetPlayerFromSlot(playerSlot);
    Console.WriteLine($"Player {playerSlot} put in server.");
    if (player == null || !player.IsValid) {
      Console.WriteLine($"Player {playerSlot} does not exist.");
      return;
    }

    var gamePlayer = converter.GetPlayer(player);
    bus.Dispatch(new PlayerJoinEvent(gamePlayer));
  }

  public void Dispose() { }
}