using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Game.Events.Body;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;
using TTT.Game.Roles;

namespace TTT.Shop.Listeners;

public class PlayerKillListener(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly IShop shop = provider.GetRequiredService<IShop>();

  [UsedImplicitly]
  [EventHandler]
  public void OnKill(PlayerDeathEvent ev) {
    if (Games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    if (ev.Killer == null) return;
    Task.Run(async () => {
      var victimBal = await shop.Load(ev.Victim);
      shop.AddBalance(ev.Killer, victimBal / 2, "Killed " + ev.Victim.Name);
    });
  }

  [UsedImplicitly]
  [EventHandler(IgnoreCanceled = true)]
  public void OnIdentify(BodyIdentifyEvent ev) {
    if (ev.Identifier == null) return;
    Task.Run(async () => {
      var victimBal = await shop.Load(ev.Body.OfPlayer);
      shop.AddBalance(ev.Identifier, victimBal / 4,
        "Identified " + ev.Body.OfPlayer.Name);

      if (ev.Body.Killer is not IOnlinePlayer killer) return;

      if (!isGoodKill(ev.Body.Killer, ev.Body.OfPlayer)) {
        var killerBal = await shop.Load(killer);
        shop.AddBalance(killer, -killerBal / 3 - victimBal / 2, "Bad Kill");
        return;
      }

      shop.AddBalance(killer, victimBal / 4, "Good Kill");
    });
  }

  private bool isGoodKill(IPlayer attacker, IPlayer victim) {
    return Roles.GetRoles(attacker).OfType<TraitorRole>().Any()
      != Roles.GetRoles(victim).OfType<TraitorRole>().Any();
  }
}