using JetBrains.Annotations;
using TTT.API.Events;
using TTT.API.Game;
using TTT.Game.Actions;
using TTT.Game.Events.Player;

namespace TTT.Game.Listeners.Loggers;

public class PlayerActionsLogger(IServiceProvider provider)
  : BaseListener(provider) {
  // Needs to be higher so we detect the kill before the game ends
  [UsedImplicitly]
  [EventHandler(Priority = Priority.HIGH)]
  public void OnPlayerKill(PlayerDeathEvent ev) {
    if (Games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    Games.ActiveGame.Logger.LogAction(new DeathAction(Provider, ev));
  }


  [UsedImplicitly]
  [EventHandler]
  public void OnPlayerDamage(PlayerDamagedEvent ev) {
    if (Games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    Games.ActiveGame.Logger.LogAction(new DamagedAction(Provider, ev));
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnPlayerAssignedRole(PlayerRoleAssignEvent ev) {
    Games.ActiveGame?.Logger.LogAction(
      new RoleAssignedAction(ev.Player, ev.Role));
  }
}