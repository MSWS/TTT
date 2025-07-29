using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using GitVersion;
using TTT.Api;
using TTT.Api.Events;
using TTT.Api.Player;
using TTT.Game.Events.Player;

namespace TTT.CS2.Listeners;

public class PlayerListeners(IEventBus bus,
  IPlayerConverter<CCSPlayerController> converter) : IPluginModule {
  public string Name => "PlayerListeners";
  public string Version => GitVersionInformation.FullSemVer;

  public void Start() { }

  public void Dispose() { }

  [GameEventHandler]
  public HookResult OnPlayerConnect(EventPlayerConnectFull ev,
    GameEventInfo _) {
    var player = ev.Userid;
    if (player == null || !player.IsValid) return HookResult.Continue;

    var gamePlayer = converter.GetPlayer(player);
    bus.Dispatch(new PlayerJoinEvent(gamePlayer));
    return HookResult.Continue;
  }

  [GameEventHandler]
  public HookResult OnPlayerDisconnect(EventPlayerDisconnect ev,
    GameEventInfo _) {
    var player = ev.Userid;
    if (player == null || !player.IsValid) return HookResult.Continue;

    var gamePlayer = converter.GetPlayer(player);
    bus.Dispatch(new PlayerLeaveEvent(gamePlayer));
    return HookResult.Continue;
  }
}