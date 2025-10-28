using CounterStrikeSharp.API.Core;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Player;
using TTT.CS2.Utils;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;

namespace TTT.CS2.Listeners;

public class MapHookListener(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  [UsedImplicitly]
  [EventHandler(Priority = Priority.MONITOR, IgnoreCanceled = true)]
  public void OnRoleAssign(PlayerRoleAssignEvent ev) {
    var player = converter.GetPlayer(ev.Player);
    if (player == null) return;
    EntityNameHelper.SetEntityName(player, ev.Role);
  }
}