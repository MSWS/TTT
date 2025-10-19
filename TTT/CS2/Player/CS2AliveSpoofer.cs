using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using JetBrains.Annotations;
using TTT.API;
using TTT.CS2.API;

namespace TTT.CS2.Player;

public class CS2AliveSpoofer : IAliveSpoofer, IPluginModule {
  private readonly HashSet<CCSPlayerController> _fakeAlivePlayers = new();
  public ISet<CCSPlayerController> FakeAlivePlayers => _fakeAlivePlayers;

  public void SpoofAlive(CCSPlayerController player) {
    if (player.IsBot) {
      Server.NextWorldUpdate(() => {
        var pawn = player.Pawn.Value;
        if (pawn == null || !pawn.IsValid) return;
        pawn.DeathTime = 0;
        Utilities.SetStateChanged(pawn, "CBasePlayerPawn", "m_flDeathTime");
        Utilities.SetStateChanged(pawn, "CBasePlayerController",
          "m_flDeathTime");

        Server.NextWorldUpdate(() => {
          player.PawnIsAlive = true;
          Utilities.SetStateChanged(player, "CCSPlayerController",
            "m_bPawnIsAlive");
        });
      });

      return;
    }

    FakeAlivePlayers.Add(player);
  }

  public void UnspoofAlive(CCSPlayerController player) {
    if (player.IsBot) {
      if (player.Pawn.Value != null && player.Pawn.Value.Health > 0) return;
      player.PawnIsAlive = false;
      Utilities.SetStateChanged(player, "CCSPlayerController",
        "m_bPawnIsAlive");
      return;
    }

    FakeAlivePlayers.Remove(player);
  }

  public void Dispose() { }
  public void Start() { }

  public void Start(BasePlugin? plugin) {
    plugin?.RegisterListener<CounterStrikeSharp.API.Core.Listeners.OnTick>(
      onTick);
  }

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnDisconnect(EventPlayerDisconnect ev) {
    if (ev.Userid == null) return HookResult.Continue;
    _fakeAlivePlayers.Remove(ev.Userid);
    return HookResult.Continue;
  }

  private void onTick() {
    _fakeAlivePlayers.RemoveWhere(p => !p.IsValid || p.Handle == IntPtr.Zero);
    foreach (var player in _fakeAlivePlayers) {
      player.PawnIsAlive = true;
      Utilities.SetStateChanged(player, "CCSPlayerController",
        "m_bPawnIsAlive");
    }
  }
}