using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using TTT.API.Events;
using TTT.API.Game;
using TTT.Game.Events.Game;
using TTT.Game.Roles;

namespace TTT.Game.Listeners;

public class TraitorBuddyInformer(IServiceProvider provider)
  : BaseListener(provider) {
  [UsedImplicitly]
  [EventHandler]
  public void OnGameStatChange(GameStateUpdateEvent ev) {
    if (ev.NewState != State.IN_PROGRESS) return;

    var traitors = ev.Game.GetAlive(typeof(TraitorRole));

    foreach (var traitor in traitors) {
      var buddies = traitors.Where(x => x != traitor).ToList();
      if (buddies.Count == 0) {
        Messenger.Message(traitor, Locale[GameMsgs.ROLE_REVEAL_TRAITORS_NONE]);
      } else {
        Messenger.Message(traitor,
          Locale[GameMsgs.ROLE_REVEAL_TRAITORS_HEADER]);
        foreach (var buddy in buddies)
          Messenger.Message(traitor,
            $" {ChatColors.Grey}- {ChatColors.Red}{buddy.Name}");
      }
    }
  }
}