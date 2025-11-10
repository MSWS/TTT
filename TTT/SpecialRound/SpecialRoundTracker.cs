using JetBrains.Annotations;
using SpecialRoundAPI;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;
using TTT.Game.Events.Game;

namespace SpecialRound;

public class SpecialRoundTracker : ISpecialRoundTracker, ITerrorModule,
  IListener {
  public List<AbstractSpecialRound> ActiveRounds { get; } = new();
  public int RoundsSinceLastSpecial { get; set; }
  public void Dispose() { }
  public void Start() { }

  [UsedImplicitly]
  [EventHandler]
  public void OnRoundEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;
    ActiveRounds.Clear();
  }
}