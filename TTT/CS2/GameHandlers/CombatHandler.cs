using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace TTT.CS2.GameHandlers;

public class CombatHandler(IServiceProvider provider) : IPluginModule {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IMessenger msg = provider.GetRequiredService<IMessenger>();
  public string Name => "CombatListeners";
  public string Version => GitVersionInformation.FullSemVer;

  public void Start() { }

  public void Dispose() { }

  /// <summary>
  /// </summary>
  /// <param name="ev"></param>
  /// <param name="_"></param>
  /// <param name="info"></param>
  /// <returns></returns>
  [UsedImplicitly]
  [GameEventHandler(HookMode.Pre)]
  public HookResult OnPlayerDeath_Pre(EventPlayerDeath ev, GameEventInfo info) {
    var player = ev.Userid;
    if (player == null) return HookResult.Continue;
    var deathEvent = new PlayerDeathEvent(converter, ev);

    Server.NextWorldUpdateAsync(() => bus.Dispatch(deathEvent));

    info.DontBroadcast = true;

    // These delays are necesary for the game engine
    Server.NextWorldUpdate(() => {
      var pawn = player.PlayerPawn.Value;
      if (pawn == null || !pawn.IsValid) return;
      pawn.DeathTime = 0;
      Utilities.SetStateChanged(pawn, "CBasePlayerPawn", "m_flDeathTime");

      Server.NextWorldUpdate(() => {
        player.PawnIsAlive = true;
        Utilities.SetStateChanged(player, "CCSPlayerController",
          "m_bPawnIsAlive");
      });
    });
    return HookResult.Continue;
  }

  [GameEventHandler]
  public HookResult OnPlayerHurt(EventPlayerHurt ev, GameEventInfo _) {
    var player = ev.Userid;
    if (player == null) return HookResult.Continue;

    var dmgEvent = new PlayerDamagedEvent(converter, ev);

    bus.Dispatch(dmgEvent);

    var pawn = player.Pawn.Value;
    if (pawn != null && pawn.IsValid) {
      pawn.Health = dmgEvent.HpLeft;
      if (player.PlayerPawn.Value != null && player.PlayerPawn.Value.IsValid)
        player.PlayerPawn.Value.ArmorValue = dmgEvent.ArmorRemaining;
    }

    if (dmgEvent.IsCanceled) return HookResult.Handled;

    return HookResult.Continue;
  }
}