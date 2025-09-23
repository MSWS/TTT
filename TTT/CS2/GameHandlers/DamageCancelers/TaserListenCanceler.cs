using TTT.API.Events;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;

namespace TTT.CS2.GameHandlers.DamageCancelers;

public class TaserListenCanceler(IServiceProvider provider)
  : BaseListener(provider) {
  [EventHandler]
  public void OnHurt(PlayerDamagedEvent ev) {
    Messenger.DebugAnnounce("PlayerDamagedEvent fired, weapon: " + ev.Weapon);
    if (ev.Weapon != "weapon_taser") return;
    ev.IsCanceled = true;
    Messenger.DebugAnnounce("Taser damage canceled");
  }
}