namespace SpecialRoundAPI;

public interface ISpecialRoundTracker {
  public AbstractSpecialRound? CurrentRound { get; set; }
  public int RoundsSinceLastSpecial { get; set; }
}