using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API;
using TTT.API.Player;

namespace TTT.CS2.Items.TeleportDecoy;

public class TeleportDecoyListener(IServiceProvider provider) : IPluginModule {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  public void Dispose() { }
  public void Start() { }

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnDecoyDetonate(EventDecoyDetonate ev, GameEventInfo _) {
    if (ev.Userid == null) return HookResult.Continue;
    var player = converter.GetPlayer(ev.Userid) as IOnlinePlayer;
    if (player == null) return HookResult.Continue;

    if (!shop.HasItem<TeleportDecoyItem>(player)) return HookResult.Continue;

    shop.RemoveItem<TeleportDecoyItem>(player);

    var vec = new Vector(ev.X, ev.Y, ev.Z + 16);
    ev.Userid.Pawn.Value?.Teleport(vec);
    return HookResult.Continue;
  }
}