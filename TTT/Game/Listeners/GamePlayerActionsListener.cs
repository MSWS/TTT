using System.Diagnostics.Tracing;
using System.Reactive.Concurrency;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.Game.Actions;
using TTT.Game.Events.Player;

namespace TTT.Game.Listeners;

public class GamePlayerActionsListener(IServiceProvider provider) : IListener {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IScheduler scheduler =
    provider.GetRequiredService<IScheduler>();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  public void Dispose() { bus.UnregisterListener(this); }

  [EventHandler]
  public void OnPlayerKill(PlayerDeathEvent ev) {
    if (!games.IsGameActive()) return;

    var game = games.ActiveGame;
    if (game == null)
      throw new InvalidOperationException(
        "Active game is null, but game is active?");

    game.Logger.LogAction(new DeathAction(ev));
  }

  [EventHandler]
  public void OnPlayerDamage(PlayerDamagedEvent ev) {
    if (!games.IsGameActive()) return;

    var game = games.ActiveGame;
    if (game == null)
      throw new InvalidOperationException(
        "Active game is null, but game is active?");

    game.Logger.LogAction(new DamagedAction(ev));
  }

  [EventHandler]
  public void OnPlayerAssignedRole(PlayerRoleAssignEvent ev) {
    if (!games.IsGameActive()) return;

    var game = games.ActiveGame;
    if (game == null)
      throw new InvalidOperationException(
        "Active game is null, but game is active?");

    game.Logger.LogAction(new RoleAssignedAction(ev.Player, ev.Role));
  }
}