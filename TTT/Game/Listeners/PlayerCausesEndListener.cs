using JetBrains.Annotations;
using TTT.API.Events;
using TTT.API.Role;
using TTT.Game.Events.Player;
using TTT.Game.Roles;

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