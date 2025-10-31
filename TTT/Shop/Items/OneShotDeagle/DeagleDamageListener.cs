using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Storage;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;
using TTT.Game.Roles;

namespace TTT.Shop.Items;

public class DeagleDamageListener(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly IShop shop = provider.GetRequiredService<IShop>();

  private OneShotDeagleConfig config
    => Provider.GetService<IStorage<OneShotDeagleConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new OneShotDeagleConfig();

  [UsedImplicitly]
  [EventHandler]
  public void OnDamage(PlayerDamagedEvent ev) {
    if (Games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    var attacker = ev.Attacker;
    var victim   = ev.Player;

    if (attacker == null) return;

    if (!shop.HasItem<OneShotDeagleItem>(attacker)) return;

    if (ev.Weapon != config.Weapon)
      // CS2 specifically causes the weapon to be "weapon_deagle" even if
      // the player is holding a revolver, so we need to check for that as well
      if (ev.Weapon != "weapon_deagle" || config.Weapon != "weapon_revolver")
        return;

    var attackerRole = Roles.GetRoles(attacker);
    var victimRole   = Roles.GetRoles(victim);

    shop.RemoveItem<OneShotDeagleItem>(attacker);
    var attackerIsTraitor = attackerRole.Any(r => r is TraitorRole);
    var victimIsTraitor   = victimRole.Any(r => r is TraitorRole);
    if (attackerIsTraitor == victimIsTraitor) {
      if (config.KillShooterOnFF) attacker.Health = 0;
      Messenger.Message(attacker, Locale[DeagleMsgs.SHOP_ITEM_DEAGLE_HIT_FF]);
      if (!config.DoesFriendlyFire) {
        ev.IsCanceled = true;
        return;
      }
    }

    ev.HpLeft = -100;
  }
}