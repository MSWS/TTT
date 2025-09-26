using JetBrains.Annotations;
using TTT.API.Events;
using TTT.API.Game;
using TTT.Game.Actions;
using TTT.Game.Events.Player;

namespace TTT.Game.Listeners.Loggers;

public class PlayerActionsLogger(IServiceProvider provider)
  : BaseListener(provider) {
  [EventHandler]
  [UsedImplicitly]
  public void OnPlayerKill(PlayerDeathEvent ev) {
    if (Games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    Games.ActiveGame.Logger.LogAction(new DeathAction(Provider, ev));
  }

  [EventHandler]
  public void OnPlayerDamage(PlayerDamagedEvent ev) {
    if (Games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    Games.ActiveGame.Logger.LogAction(new DamagedAction(Provider, ev));
  }

  [EventHandler]
  public void OnPlayerAssignedRole(PlayerRoleAssignEvent ev) {
    if (Games.ActiveGame is not { State: State.IN_PROGRESS }) return;
    Games.ActiveGame.Logger.LogAction(
      new RoleAssignedAction(ev.Player, ev.Role));
  }
}