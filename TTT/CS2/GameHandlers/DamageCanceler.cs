using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;

namespace TTT.CS2.GameHandlers;

public class DamageCanceler(IServiceProvider provider)
  : IPluginModule, IListener {
  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  public void Dispose() { }

  public string Name => nameof(DamageCanceler);
  public string Version => GitVersionInformation.FullSemVer;

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

    if (attacker == null || !attacker.IsValid
      || games.ActiveGame is not { State: State.IN_PROGRESS })
      return HookResult.Continue;
    return HookResult.Handled;
  }
}