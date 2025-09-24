using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.CS2.lang;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;

namespace TTT.CS2.GameHandlers.DamageCancelers;

public class TaserListenCanceler(IServiceProvider provider)
  : BaseListener(provider) {
  [EventHandler]
  public void OnHurt(PlayerDamagedEvent ev) {
    if (Games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    if (ev.Weapon == null) return;
    if (!ev.Weapon.Contains("taser", StringComparison.OrdinalIgnoreCase))
      return;
    ev.IsCanceled = true;

    var victim   = ev.Player;
    var attacker = ev.Attacker;

    if (attacker == null) return;

    Messenger.Message(attacker,
      Locale[CS2Msgs.TASER_SCANNED(victim, Roles.GetRoles(victim).First())]);
  }
}