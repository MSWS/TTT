using CounterStrikeSharp.API;
using JetBrains.Annotations;
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

  [UsedImplicitly]
  [EventHandler]
  public void OnDamage(PlayerDamagedEvent ev) {
    Messenger.Debug("DeagleDamageListener: OnDamage");
    if (Games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    var attacker = ev.Attacker;
    var victim   = ev.Player;

    if (attacker == null) return;

    var deagleItem = shop.GetOwnedItems(attacker)
     .FirstOrDefault(s => s.Id == OneShotDeagle.ID);
    if (deagleItem == null) return;

    if (ev.Weapon != config.Weapon) {
      // CS2 specifically causes the weapon to be "weapon_deagle" even if
      // the player is holding a revolver, so we need to check for that as well
      if (ev.Weapon is not "weapon_deagle"
        || !config.Weapon.Equals("weapon_revolver"))
        return;
    }

    var attackerRole = Roles.GetRoles(attacker);
    var victimRole   = Roles.GetRoles(victim);

    shop.RemoveItem(attacker, deagleItem);
    if (!config.DoesFriendlyFire && attackerRole.Intersect(victimRole).Any()) {
      Messenger.DebugAnnounce(
        "DeagleDamageListener: Friendly fire is off, roles intersect");
      return;
    }

    if (victim is not IOnlinePlayer onlineVictim) return;
    onlineVictim.Health = 0;
  }
}