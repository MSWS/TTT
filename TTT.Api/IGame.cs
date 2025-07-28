namespace TTT.Api;

public interface IGame {
  ICollection<IPlayer> Players { get; }
  DateTime StartedAt { get; }
  DateTime? FinishedAt { get; }
  IList<IAction> Actions { get; }
}