using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Events;
using TTT.API.Game;
using TTT.CS2.lang;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;

namespace TTT.Shop.Listeners;

public class TaseRewarder(IServiceProvider provider) : BaseListener(provider) {
  private readonly IShop shop = provider.GetRequiredService<IShop>();

  [UsedImplicitly]
  [EventHandler]
  public void OnHurt(PlayerDamagedEvent ev) {
    if (Games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    if (ev.Weapon == null) return;
    if (!ev.Weapon.Contains("taser", StringComparison.OrdinalIgnoreCase))
      return;
    ev.IsCanceled = true;

    var attacker = ev.Attacker;

    if (attacker == null) return;

    shop.AddBalance(attacker, 30, "Successful Tase");
  }
}