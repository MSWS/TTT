namespace TTT.API;

public enum State {
  /// <summary>
  ///   Waiting for players to join.
  /// </summary>
  WAITING,

  /// <summary>
  ///   Waiting for the countdown to finish before starting the game.
  /// </summary>
  COUNTDOWN,

  /// <summary>
  ///   Currently playing the game.
  /// </summary>
  IN_PROGRESS,

  /// <summary>
  ///   Game has finished.
  /// </summary>
  FINISHED
}