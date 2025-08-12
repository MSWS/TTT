using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.API.Role;
using TTT.Game.Events.Player;
using TTT.Game.Roles;

namespace TTT.Game.Listeners;

public class PlayerCausesEndListener(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly IMessenger msg = provider.GetRequiredService<IMessenger>();
  public override string Name { get; } = nameof(PlayerCausesEndListener);

  [EventHandler]
  [UsedImplicitly]
  public void OnKill(PlayerDeathEvent ev) {
    if (!Games.IsGameActive()) return;
    var endGame = getWinningTeam(out var winningTeam);

    if (!endGame) return;

    if (winningTeam == null)
      Games.ActiveGame?.EndGame(new EndReason("Draw"));
    else
      Games.ActiveGame?.EndGame(new EndReason(winningTeam));
  }

  [EventHandler]
  [UsedImplicitly]
  public void OnLeave(PlayerLeaveEvent ev) {
    if (!Games.IsGameActive()) return;
    var endGame = getWinningTeam(out var winningTeam);

    if (!endGame) return;

    if (winningTeam == null)
      Games.ActiveGame?.EndGame(new EndReason("Draw"));
    else
      Games.ActiveGame?.EndGame(new EndReason(winningTeam));
  }

  private bool getWinningTeam(out IRole? winningTeam) {
    var game = Games.ActiveGame;
    winningTeam = null;
    if (game is null) return false;

    var traitorRole =
      game.Roles.First(r => r.GetType().IsAssignableTo(typeof(TraitorRole)));
    var innocentRole =
      game.Roles.First(r => r.GetType().IsAssignableTo(typeof(InnocentRole)));
    var detectiveRole = game.Roles.First(r
      => r.GetType().IsAssignableTo(typeof(DetectiveRole)));

    var traitorsAlive    = game.GetAlive(typeof(TraitorRole)).Count;
    var nonTraitorsAlive = game.GetAlive().Count - traitorsAlive;
    var detectivesAlive  = game.GetAlive(typeof(DetectiveRole)).Count;

    msg.DebugAnnounce($"Traitors alive: {traitorsAlive}, "
      + $"Non-traitors alive: {nonTraitorsAlive}, "
      + $"Detectives alive: {detectivesAlive}");

    switch (traitorsAlive) {
      case 0 when nonTraitorsAlive == 0:
        winningTeam = null;
        return true;
      case > 0 when nonTraitorsAlive == 0:
        winningTeam = traitorRole;
        return true;
      case 0 when nonTraitorsAlive > 0:
        winningTeam = nonTraitorsAlive == detectivesAlive ?
          detectiveRole :
          innocentRole;
        return true;
      default:
        winningTeam = null;
        return false;
    }
  }
}