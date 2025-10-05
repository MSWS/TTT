using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Game.Events.Body;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;

namespace TTT.Shop.Listeners;

public class PlayerKillListener(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly IShop shop = provider.GetRequiredService<IShop>();

  [UsedImplicitly]
  [EventHandler]
  public async Task OnKill(PlayerDeathEvent ev) {
    if (Games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    if (ev.Killer == null) return;
    var victimBal = await shop.Load(ev.Victim);

    shop.AddBalance(ev.Killer, victimBal / 6, "Killed " + ev.Victim.Name);
  }

  [UsedImplicitly]
  [EventHandler(IgnoreCanceled = true)]
  public async Task OnIdentify(BodyIdentifyEvent ev) {
    if (ev.Identifier == null) return;
    var victimBal = await shop.Load(ev.Body.OfPlayer);
    shop.AddBalance(ev.Identifier, victimBal / 4,
      "Identified " + ev.Body.OfPlayer.Name);

    if (ev.Body.Killer is not IOnlinePlayer killer) return;

    if (!isGoodKill(ev.Body.Killer, ev.Body.OfPlayer)) {
      var killerBal = await shop.Load(killer);
      shop.AddBalance(killer, -killerBal / 4 - victimBal / 2, "Bad Kill");
      return;
    }

    shop.AddBalance(killer, victimBal / 4, "Good Kill");
  }

  private bool isGoodKill(IPlayer attacker, IPlayer victim) {
    return !Roles.GetRoles(attacker).Intersect(Roles.GetRoles(victim)).Any();
  }
}