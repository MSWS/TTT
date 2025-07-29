using TTT.Api.Player;
using TTT.Game;

namespace TTT.Api;

public interface IGame {
  /// <summary>
  ///   The list of players in the game.
  ///   Spectators are not included in this list.
  /// </summary>
  ICollection<IPlayer> Players { get; }

  DateTime? StartedAt { get; }
  DateTime? FinishedAt { get; }
  SortedDictionary<DateTime, ISet<IAction>> Actions { get; }

  State State { get; set; }

  /// <summary>
  ///   Attempts to start a game.
  ///   Depending on implementation, this may start a countdown or immediately start the game.
  /// </summary>
  /// <param name="countdown"></param>
  IObservable<long> Start(TimeSpan countdown);
}