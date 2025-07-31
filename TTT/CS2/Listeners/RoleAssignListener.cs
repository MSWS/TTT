using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using TTT.API.Events;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace TTT.CS2.Listeners;

public class RoleAssignListener(IEventBus bus,
  IPlayerConverter<CCSPlayerController> players) : IListener {
  public void Dispose() { bus.UnregisterListener(this); }

  [EventHandler(IgnoreCanceled = true)]
  public void OnAssigned(PlayerRoleAssignEvent ev) {
    var player = players.GetPlayer(ev.Player);
    if (player == null || !player.IsValid) return;

    if (player.Team == CsTeam.Spectator) ev.IsCanceled = true;
  }
}