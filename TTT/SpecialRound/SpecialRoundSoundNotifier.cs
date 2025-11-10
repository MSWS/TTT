using CounterStrikeSharp.API;
using JetBrains.Annotations;
using SpecialRound.Events;
using TTT.API.Events;
using TTT.Game.Listeners;

namespace SpecialRound;

public class SpecialRoundSoundNotifier(IServiceProvider provider)
  : BaseListener(provider) {
  [UsedImplicitly]
  [EventHandler]
  public void OnSpecialRoundStart(SpecialRoundStartEvent ev) {
    foreach (var player in Utilities.GetPlayers())
      player.EmitSound("UI.XP.Star.Spend", null, 0.2f);
  }
}