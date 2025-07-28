using TTT.Api.Player;

namespace TTT.Api;

public interface IGame {
  ICollection<IPlayer> Players { get; }
  DateTime StartedAt { get; }
  DateTime? FinishedAt { get; }
  SortedDictionary<DateTime, ISet<IAction>> Actions { get; }
}