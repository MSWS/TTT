using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Player;
using TTT.Game.Events.Game;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;
using TTT.Game.Roles;

namespace TTT.CS2.Listeners;

public class MapHookListener(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  [UsedImplicitly]
  [EventHandler(Priority = Priority.MONITOR, IgnoreCanceled = true)]
  public void OnRoleAssign(PlayerRoleAssignEvent ev) {
    Server.NextWorldUpdate(() => {
      var player = converter.GetPlayer(ev.Player);
      if (player == null) return;

      switch (ev.Role) {
        case TraitorRole:
          player.Pawn.Value?.AcceptInput("AddContext", null, null, "TRAITOR:1");
          break;
        case DetectiveRole:
          player.Pawn.Value?.AcceptInput("AddContext", null, null,
            "DETECTIVE:1");
          break;
        case InnocentRole:
          player.Pawn.Value?.AcceptInput("AddContext", null, null,
            "INNOCENT:1");
          break;
      }
    });
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnRoundEnd(GameInitEvent ev) {
    foreach (var player in Utilities.GetPlayers()) {
      if (player.Pawn.Value == null) continue;
      player.Pawn.Value.AcceptInput("RemoveContext", null, null, "TRAITOR");
      player.Pawn.Value.AcceptInput("RemoveContext", null, null, "DETECTIVE");
      player.Pawn.Value.AcceptInput("RemoveContext", null, null, "INNOCENT");
    }
  }
}