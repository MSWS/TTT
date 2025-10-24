using SpecialRoundAPI;

namespace SpecialRound;

public interface ISpecialRoundTracker {
  public AbstractSpecialRound? CurrentRound { get; set; }
  public int RoundsSinceLastSpecial { get; set; }
}