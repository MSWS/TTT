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
    Messenger.DebugAnnounce("Kill event for shop");
    if (Games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    Messenger.DebugAnnounce("Game in progress");
    if (ev.Killer == null) return;
    Messenger.DebugAnnounce("Killer not null");
    var victimBal = await shop.Load(ev.Victim);
    Messenger.DebugAnnounce("Victim balance loaded: " + victimBal);

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
      shop.AddBalance(killer, -killerBal / 4,
        ev.Body.OfPlayer.Name + " kill invalidated");
      return;
    }

    shop.AddBalance(killer, victimBal / 4,
      ev.Body.OfPlayer.Name + " kill validated");
  }

  private bool isGoodKill(IPlayer attacker, IPlayer victim) {
    return !Roles.GetRoles(attacker).Intersect(Roles.GetRoles(victim)).Any();
  }
}