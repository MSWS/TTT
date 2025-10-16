using System.Reactive.Concurrency;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs.Traitor;
using TTT.API;
using TTT.API.Player;
using TTT.API.Role;
using TTT.API.Storage;

namespace TTT.CS2.Items.ClusterGrenade;

public class ClusterGrenadeListener(IServiceProvider provider) : IPluginModule {
  private readonly ClusterGrenadeConfig config =
    provider.GetService<IStorage<ClusterGrenadeConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new ClusterGrenadeConfig();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnHeGrenade(EventHegrenadeDetonate ev, GameEventInfo _) {
    if (ev.Userid == null) return HookResult.Continue;
    var player = converter.GetPlayer(ev.Userid) as IOnlinePlayer;
    if (player == null) return HookResult.Continue;
    if (!shop.HasItem<ClusterGrenadeItem>(player)) return HookResult.Continue;

    shop.RemoveItem<ClusterGrenadeItem>(player);

    for (var i = 0; i < config.GrenadeCount; i++) {
      var entity =
        Utilities.GetEntityFromIndex<CHEGrenadeProjectile>(ev.Entityid);

      if (entity == null || entity.AbsOrigin == null) continue;

      // Throw grenade in circular pattern
      var angle = new Vector(
        (float)(Math.Cos(2 * Math.PI / config.GrenadeCount * i)
          * config.ThrowForce),
        (float)(Math.Sin(2 * Math.PI / config.GrenadeCount * i)
          * config.ThrowForce), config.UpForce);

      if (ev.Userid.Pawn.Value == null) continue;

      GrenadeDataHelper.CreateGrenade(entity.AbsOrigin, QAngle.Zero, angle,
        Vector.Zero, ev.Userid.Pawn.Value.Handle, ev.Userid.Team);
    }

    return HookResult.Continue;
  }

  public void Dispose() { }
  public void Start() { }
}