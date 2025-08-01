using TTT.API.Events;
using TTT.Game.Actions;
using TTT.Game.Events.Player;

namespace TTT.Game.Listeners;

public class GamePlayerActionsListener(IServiceProvider provider)
  : BaseListener(provider) {
  public override string Name => nameof(GamePlayerActionsListener);

  [EventHandler]
  public void OnPlayerKill(PlayerDeathEvent ev) {
    if (!Games.IsGameActive()) return;

    var game = Games.ActiveGame;
    if (game == null)
      throw new InvalidOperationException(
        "Active game is null, but game is active?");

    game.Logger.LogAction(new DeathAction(ev));
  }

  [EventHandler]
  public void OnPlayerDamage(PlayerDamagedEvent ev) {
    if (!Games.IsGameActive()) return;

    var game = Games.ActiveGame;
    if (game == null)
      throw new InvalidOperationException(
        "Active game is null, but game is active?");

    game.Logger.LogAction(new DamagedAction(ev));
  }

  [EventHandler]
  public void OnPlayerAssignedRole(PlayerRoleAssignEvent ev) {
    if (!Games.IsGameActive()) return;

    var game = Games.ActiveGame;
    if (game == null)
      throw new InvalidOperationException(
        "Active game is null, but game is active?");

    game.Logger.LogAction(new RoleAssignedAction(ev.Player, ev.Role));
  }
}