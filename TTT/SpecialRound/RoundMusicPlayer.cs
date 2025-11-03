using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using SpecialRound.Events;
using TTT.API.Events;
using TTT.Game.Listeners;

namespace SpecialRound;

public class RoundMusicPlayer(IServiceProvider provider)
  : BaseListener(provider) {
  [UsedImplicitly]
  [EventHandler(Priority = Priority.MONITOR, IgnoreCanceled = true)]
  public void OnSpecialRound(SpecialRoundStartEvent ev) {
    foreach (var player in Utilities.GetPlayers()) {
      player.EmitSound("UI.DeathMatchBonusAlertEnd",
        new RecipientFilter(player));
    }
  }
}