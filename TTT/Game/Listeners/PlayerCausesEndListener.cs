using JetBrains.Annotations;
using TTT.API.Events;
using TTT.Game.Events.Player;

namespace TTT.Game.Listeners;

public class PlayerCausesEndListener(IServiceProvider provider)
  : BaseListener(provider) {
  [EventHandler]
  [UsedImplicitly]
  public void OnKill(PlayerDeathEvent ev) {
    Games.ActiveGame?.CheckEndConditions();
  }

  [EventHandler]
  [UsedImplicitly]
  public void OnLeave(PlayerLeaveEvent ev) {
    Games.ActiveGame?.CheckEndConditions();
  }
}