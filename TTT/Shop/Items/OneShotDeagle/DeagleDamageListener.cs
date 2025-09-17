using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;
using TTT.API.Storage;
using TTT.Game.Events.Player;

namespace TTT.Shop.Items;

public class DeagleDamageListener(IServiceProvider provider) : IListener {
  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  private readonly IShop shop = provider.GetRequiredService<IShop>();

  private readonly OneShotDeagleConfig config =
    provider.GetService<IStorage<OneShotDeagleConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new OneShotDeagleConfig();

  public void Dispose() { }

  [EventHandler]
  public void OnDamage(PlayerDamagedEvent ev) {
    if (games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    var attacker = ev.Attacker;
    var victim   = ev.Player;

    if (attacker == null) return;
    var deagleItem = shop.GetOwnedItems(attacker)
     .FirstOrDefault(s => s.Id == OneShotDeagle.ID);
    if (deagleItem == null) return;

    if (ev.Weapon != config.Weapon) return;

    var attackerRole = roles.GetRoles(attacker);
    var victimRole   = roles.GetRoles(victim);

    shop.RemoveItem(attacker, deagleItem);
    if (!config.DoesFriendlyFire && attackerRole.Intersect(victimRole).Any())
      return;

    if (victim is not IOnlinePlayer onlineVictim) return;
    onlineVictim.Health = 0;
  }
}