using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace TTT.CS2.GameHandlers;

public class DamageCanceler(IServiceProvider provider) : IPluginModule {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  public void Dispose() {
    if (OperatingSystem.IsWindows()) return;
    VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Unhook(onTakeDamage,
      HookMode.Pre);
  }

  public void Start() { }

  public void Start(BasePlugin? plugin) {
    if (OperatingSystem.IsWindows()) return;
    VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(onTakeDamage,
      HookMode.Pre);
  }

  private HookResult onTakeDamage(DynamicHook hook) {
    Server.PrintToChatAll("DamageCanceler: onTakeDamage called");
    var damagedEvent = new PlayerDamagedEvent(converter, hook);

    Server.PrintToChatAll("DamageCanceler: Dispatching PlayerDamagedEvent");
    _ = bus.Dispatch(damagedEvent);
    Server.PrintToChatAll("DamageCanceler: Setting damage to "
      + damagedEvent.DmgDealt);
    Server.PrintToChatAll(
      "DamageCanceler: Dispatched PlayerDamagedEvent, is canceled="
      + damagedEvent.IsCanceled);


    if (damagedEvent.IsCanceled) return HookResult.Handled;

    if (damagedEvent.HpLeft == 0) {
      var playerPawn = hook.GetParam<CCSPlayerPawn>(0);
      var player     = playerPawn.Controller.Value?.As<CCSPlayerController>();
      if (player == null || !player.IsValid) {
        Server.PrintToChatAll("DamageCanceler: Player is null or invalid");
        return HookResult.Continue;
      }

      player.CommitSuicide(false, true);
    }

    var info = hook.GetParam<CTakeDamageInfo>(1);
    info.Damage = damagedEvent.DmgDealt;
    return HookResult.Continue;
  }
}