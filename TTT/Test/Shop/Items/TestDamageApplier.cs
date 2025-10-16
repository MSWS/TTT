using JetBrains.Annotations;
using TTT.API.Events;
using TTT.API.Player;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;

namespace TTT.Test.Shop.Items;

public class TestDamageApplier(IServiceProvider provider)
  : BaseListener(provider) {
  [UsedImplicitly]
  [EventHandler(Priority = Priority.MONITOR)]
  public void OnDamage(PlayerDamagedEvent ev) {
    if (ev.Player is not IOnlinePlayer online) return;

    online.Health = Math.Clamp(ev.HpLeft, 0, online.MaxHealth);
  }
}