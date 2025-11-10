namespace SpecialRoundAPI;

public interface ISpecialRoundTracker {
  public List<AbstractSpecialRound> ActiveRounds { get; }

  public int RoundsSinceLastSpecial { get; set; }
}