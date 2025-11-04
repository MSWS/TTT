using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using SpecialRound.lang;
using SpecialRoundAPI;
using SpecialRoundAPI.Configs;
using TTT.API;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Game.Events.Game;
using TTT.Locale;

namespace SpecialRound.Rounds;

public class PistolRound(IServiceProvider provider)
  : AbstractSpecialRound(provider), IPluginModule {
  public override string Name => "Pistol";
  public override IMsg Description => RoundMsgs.SPECIAL_ROUND_PISTOL;
  private BasePlugin? plugin;

  private PistolRoundConfig config
    => Provider.GetService<IStorage<PistolRoundConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new PistolRoundConfig();

  private IInventoryManager inventory = provider
   .GetRequiredService<IInventoryManager>();

  public override SpecialRoundConfig Config => config;

  public void Start(BasePlugin? newPluing) { plugin = newPluing; }

  public override void ApplyRoundEffects() {
    VirtualFunctions.CCSPlayer_ItemServices_CanAcquireFunc.Hook(canAcquire,
      HookMode.Pre);
    foreach (var player in Finder.GetOnline())
      inventory.RemoveWeaponInSlot(player, 0);
  }

  private HookResult canAcquire(DynamicHook hook) {
    var player = hook.GetParam<CCSPlayer_ItemServices>(0)
     .Pawn.Value.Controller.Value?.As<CCSPlayerController>();
    var data = VirtualFunctions.GetCSWeaponDataFromKey.Invoke(-1,
      hook.GetParam<CEconItemView>(1).ItemDefinitionIndex.ToString());

    if (player == null || !player.IsValid) return HookResult.Continue;

    if (Tag.RIFLES.Contains(data.Name)) {
      hook.SetReturn(AcquireResult.NotAllowedByMode);
      return HookResult.Handled;
    }

    return HookResult.Continue;
  }

  public override void OnGameState(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;

    VirtualFunctions.CCSPlayer_ItemServices_CanAcquireFunc.Unhook(canAcquire,
      HookMode.Pre);
  }
}