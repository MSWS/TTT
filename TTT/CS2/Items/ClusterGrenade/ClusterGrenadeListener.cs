using System.Reactive.Concurrency;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
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

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly List<IDisposable> poisonSmokes = [];

  private readonly IRoleAssigner roleAssigner =
    provider.GetRequiredService<IRoleAssigner>();

  private readonly IScheduler scheduler =
    provider.GetRequiredService<IScheduler>();

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnHeGrenade(EventHegrenadeDetonate ev, GameEventInfo _) {
    if (ev.Userid == null) return HookResult.Continue;
    var player = converter.GetPlayer(ev.Userid) as IOnlinePlayer;
    if (player == null) return HookResult.Continue;
    if (!shop.HasItem<ClusterGrenadeItem>(player)) return HookResult.Continue;

    shop.RemoveItem<ClusterGrenadeItem>(player);
    // Utilities.CreateEntityByName<>()

    Server.PrintToChatAll("[TTT] Cluster Grenade detonated!");
    return HookResult.Continue;
  }

  public void Dispose() { }
  public void Start() { }
}