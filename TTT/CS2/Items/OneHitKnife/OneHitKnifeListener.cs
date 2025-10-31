using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Configs.Traitor;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Storage;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;

namespace TTT.CS2.Items.OneHitKnife;

public class OneHitKnifeListener(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly IShop shop = provider.GetRequiredService<IShop>();

  private OneHitKnifeConfig config
    => Provider.GetService<IStorage<OneHitKnifeConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new OneHitKnifeConfig();

  [UsedImplicitly]
  [EventHandler]
  public void OnDamage(PlayerDamagedEvent ev) {
    if (Games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    if (ev.Weapon == null || !Tag.KNIVES.Contains(ev.Weapon)) return;

    var attacker = ev.Attacker;
    var victim   = ev.Player;

    if (attacker == null) return;
    if (!shop.HasItem<OneHitKnife>(attacker)) return;

    var friendly = Roles.GetRoles(attacker)
     .Any(r => Roles.GetRoles(victim).Contains(r));
    if (friendly && !config.FriendlyFire) return;

    shop.RemoveItem<OneHitKnife>(attacker);
    ev.HpLeft = -100;
  }
}