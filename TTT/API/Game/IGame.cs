using TTT.API.Player;
using TTT.API.Role;

namespace TTT.API.Game;

public interface IGame : IDisposable {
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
  IObservable<long>? Start(TimeSpan? countdown = null);

  void EndGame(IRole? winningRole = null);

  bool IsInProgress() { return State is State.COUNTDOWN or State.IN_PROGRESS; }

  ISet<IOnlinePlayer> GetAlive() {
    return Players.OfType<IOnlinePlayer>().Where(p => p.IsAlive).ToHashSet();
  }

  int GetAlive(Type roleType) {
    if (!typeof(IRole).IsAssignableFrom(roleType))
      throw new ArgumentException(
        "roleType must be a type that implements IRole", nameof(roleType));

    return GetAlive().Count(p => p.Roles.Any(r => r.GetType() == roleType));
  }

  int GetAlive(IRole role) { return GetAlive(role.GetType()); }
}