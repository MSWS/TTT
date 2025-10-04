using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using TTT.API;
using TTT.CS2.Extensions;
using TTT.CS2.RayTrace.Class;
using TTT.CS2.RayTrace.Enum;

namespace TTT.CS2.GameHandlers;

public class NameDisplayer : IPluginModule {
  public void Dispose() { }
  public void Start() { }

  public void Start(BasePlugin? plugin) {
    plugin?.AddTimer(0.25f, showNames, TimerFlags.REPEAT);
  }

  private void showNames() {
    foreach (var player in Utilities.GetPlayers()) {
      if (player.GetHealth() <= 0) continue;

      var target = player.GetGameTraceByEyePosition(TraceMask.MaskSolid,
        Contents.NoDraw, player);

      if (target == null) continue;

      if (!target.Value.HitPlayer(out var hit)) continue;
      if (hit == null) continue;

      player.PrintToCenterAlert($"{hit.PlayerName}");
    }
  }
}