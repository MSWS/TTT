using System.Reactive.Linq;
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

  /// <summary>
  ///   Attempts to start a game.
  ///   Depending on implementation, this may start a countdown or immediately start the game.
  /// </summary>
  IObservable<long> Start();
  
  State State { get; set; }
}