using TTT.API.Player;

namespace TTT.API.Game;

public interface IActionLogger {
  /// <summary>
  ///   Logs an action that has occurred in the game.
  /// </summary>
  /// <param name="action">The action to log.</param>
  void LogAction(IAction action);

  IEnumerable<(DateTime, IAction)> GetActions();

  /// <summary>
  ///   Clears all logged actions.
  /// </summary>
  void ClearActions();

  void PrintLogs();
  void PrintLogs(IOnlinePlayer? player);
  
  string[] MakeLogs();
}