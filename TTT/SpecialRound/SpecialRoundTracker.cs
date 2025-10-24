using SpecialRoundAPI;
using TTT.API;

namespace SpecialRound;

public class SpecialRoundTracker : ISpecialRoundTracker, ITerrorModule {
  public AbstractSpecialRound? CurrentRound { get; set; }
  public int RoundsSinceLastSpecial { get; set; }
  public void Dispose() { }
  public void Start() { }
}