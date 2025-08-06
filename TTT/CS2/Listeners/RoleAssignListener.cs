using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Player;
using TTT.CS2.Extensions;
using TTT.CS2.Roles;
using TTT.Game.Events.Player;

namespace TTT.CS2.Listeners;

public class RoleAssignListener(IServiceProvider provider) : IListener {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerConverter<CCSPlayerController> players =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  public void Dispose() { bus.UnregisterListener(this); }

  [EventHandler(IgnoreCanceled = true)]
  public void OnAssigned(PlayerRoleAssignEvent ev) {
    var player = players.GetPlayer(ev.Player);
    if (player == null || !player.IsValid) return;

    if (player.Team == CsTeam.Spectator) {
      ev.Role = new SpectatorRole(provider);
      return;
    }

    player.SwitchTeam(ev.Role is CS2DetectiveRole ?
      CsTeam.CounterTerrorist :
      CsTeam.Terrorist);

    if (ev.Role is not CS2DetectiveRole) return;

    player.SetClan(ev.Role.Name);
    var levelChange = new EventNextlevelChanged(true);
    levelChange.FireEvent(false);
  }
}