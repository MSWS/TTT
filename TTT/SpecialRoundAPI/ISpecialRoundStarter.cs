namespace SpecialRoundAPI;

public interface ISpecialRoundStarter {
  /// <summary>
  ///   Attempts to start the given special round.
  ///   Will bypass most checks, but may still return null if starting the round
  ///   is not possible.
  /// </summary>
  /// <param name="round"></param>
  /// <returns></returns>
  public List<AbstractSpecialRound>? TryStartSpecialRound(
    List<AbstractSpecialRound>? round = null);
}