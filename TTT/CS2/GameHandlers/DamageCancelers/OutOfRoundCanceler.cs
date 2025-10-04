using JetBrains.Annotations;
using TTT.API.Events;
using TTT.API.Game;
using TTT.CS2.Utils;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;

namespace TTT.CS2.GameHandlers.DamageCancelers;

public class OutOfRoundCanceler(IServiceProvider provider)
  : BaseListener(provider) {
  [UsedImplicitly]
  [EventHandler]
  public void OnHurt(PlayerDamagedEvent ev) {
    if (RoundUtil.IsWarmup()) return;
    if (Games.ActiveGame is not { State: State.IN_PROGRESS or State.FINISHED })
      ev.IsCanceled = true;
  }
}