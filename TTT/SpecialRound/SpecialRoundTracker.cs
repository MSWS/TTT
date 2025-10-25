using JetBrains.Annotations;
using SpecialRoundAPI;
using TTT.API;
using TTT.API.Events;
using TTT.Game.Events.Game;

namespace SpecialRound;

public class SpecialRoundTracker : ISpecialRoundTracker, ITerrorModule,
  IListener {
  public AbstractSpecialRound? CurrentRound { get; set; }
  public int RoundsSinceLastSpecial { get; set; }
  public void Dispose() { }
  public void Start() { }

  [UsedImplicitly]
  [EventHandler]
  public void OnRoundEnd(GameStateUpdateEvent ev) { CurrentRound = null; }
}