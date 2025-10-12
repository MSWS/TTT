using JetBrains.Annotations;
using TTT.API.Events;
using TTT.Game.Events.Player;

namespace TTT.Game.Listeners;

public class PlayerDeathInformer(IServiceProvider provider)
  : BaseListener(provider) {
  [UsedImplicitly]
  [EventHandler]
  public void OnDeath(PlayerDeathEvent ev) {
    if (ev.Killer == null) return;
    var killerRole = Roles.GetRoles(ev.Killer).FirstOrDefault();
    if (killerRole == null) return;
    Messenger.Message(ev.Victim,
      Locale[GameMsgs.ROLE_REVEAL_DEATH(killerRole)]);
  }
}