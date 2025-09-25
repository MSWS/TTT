using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace TTT.CS2.GameHandlers;

public class PlayerConnectionsHandler(IServiceProvider provider)
  : IPluginModule {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();


  public void Start() { }

  public void Start(BasePlugin? plugin, bool hotReload) {
    plugin
    ?.RegisterListener<CounterStrikeSharp.API.Core.Listeners.OnClientConnected>(
        connectToServer);
    plugin
    ?.RegisterListener<
        CounterStrikeSharp.API.Core.Listeners.OnClientDisconnect>(
        disconnectFromServer);

    Server.NextWorldUpdate(() => {
      foreach (var ev in Utilities.GetPlayers()
       .Select(player => converter.GetPlayer(player))
       .Select(gamePlayer => new PlayerJoinEvent(gamePlayer))) {
        bus.Dispatch(ev);
      }
    });
  }

  public void Dispose() { }

  private void disconnectFromServer(int playerSlot) {
    var player = Utilities.GetPlayerFromSlot(playerSlot);
    if (player == null || !player.IsValid) return;

    var gamePlayer = converter.GetPlayer(player);
    Server.NextWorldUpdate(()
      => bus.Dispatch(new PlayerLeaveEvent(gamePlayer)));
  }

  private void connectToServer(int playerSlot) {
    var player = Utilities.GetPlayerFromSlot(playerSlot);
    Console.WriteLine($"Player {playerSlot} put in server.");
    if (player == null || !player.IsValid) {
      Console.WriteLine($"Player {playerSlot} does not exist.");
      return;
    }

    var gamePlayer = converter.GetPlayer(player);
    bus.Dispatch(new PlayerJoinEvent(gamePlayer));
  }
}