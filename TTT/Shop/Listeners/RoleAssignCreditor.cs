using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using TTT.API.Events;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;

namespace TTT.Shop.Listeners;

public class RoleAssignCreditor(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly ShopConfig config =
    provider.GetService<IStorage<ShopConfig>>()?.Load().GetAwaiter().GetResult()
    ?? new ShopConfig(provider);

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  [UsedImplicitly]
  [EventHandler]
  public void OnRoleAssign(PlayerRoleAssignEvent ev) {
    var toGive = config.StartingCreditsForRole(ev.Role);
    if (ev.Player is not IOnlinePlayer online) return;
    shop.AddBalance(online, toGive, "Round Start", false);
  }
}