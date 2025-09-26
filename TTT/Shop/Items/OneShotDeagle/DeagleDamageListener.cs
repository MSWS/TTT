using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;

namespace TTT.Shop.Items;

public class DeagleDamageListener(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly OneShotDeagleConfig config =
    provider.GetService<IStorage<OneShotDeagleConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new OneShotDeagleConfig();

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  [EventHandler]
  public void OnDamage(PlayerDamagedEvent ev) {
    Messenger.Debug("DeagleDamageListener: OnDamage");
    if (Games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    var attacker = ev.Attacker;
    var victim   = ev.Player;

    if (attacker == null) return;

    Messenger.Debug("DeagleDamageListener: Attacker is not null");

    var deagleItem = shop.GetOwnedItems(attacker)
     .FirstOrDefault(s => s.Id == OneShotDeagle.ID);
    if (deagleItem == null) return;

    Messenger.DebugAnnounce(
      $"DeagleDamageListener: Attacker has deagle item, weapon: {ev.Weapon}");

    if (ev.Weapon != config.Weapon) return;

    var attackerRole = Roles.GetRoles(attacker);
    var victimRole   = Roles.GetRoles(victim);

    shop.RemoveItem(attacker, deagleItem);
    if (!config.DoesFriendlyFire && attackerRole.Intersect(victimRole).Any())
      return;

    if (victim is not IOnlinePlayer onlineVictim) return;
    onlineVictim.Health = 0;
  }
}