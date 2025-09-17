using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace TTT.CS2.GameHandlers;

public class DamageCanceler(IServiceProvider provider) : IPluginModule {
  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  public void Dispose() { }

  public void Start() { }

  public void Start(BasePlugin? plugin) {
    if (OperatingSystem.IsWindows()) return;
    VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(onTakeDamage,
      HookMode.Pre);
  }

  private HookResult onTakeDamage(DynamicHook hook) {
    var info       = hook.GetParam<CTakeDamageInfo>(1);
    var playerPawn = hook.GetParam<CCSPlayerPawn>(0);

    var player = playerPawn.Controller.Value?.As<CCSPlayerController>();
    if (player == null || !player.IsValid) return HookResult.Continue;

    var attackerPawn = info.Attacker;
    var attacker     = attackerPawn.Value?.As<CCSPlayerController>();

    var damagedEvent = new PlayerDamagedEvent(converter, hook);

    bus.Dispatch(damagedEvent);

    if (damagedEvent.IsCanceled) return HookResult.Handled;

    if (damagedEvent.HpModified) {
      if (damagedEvent.Player is not IOnlinePlayer onlinePlayer)
        return HookResult.Continue;
      onlinePlayer.Health = damagedEvent.HpLeft;
      return HookResult.Handled;
    }

    return HookResult.Continue;
  }
}