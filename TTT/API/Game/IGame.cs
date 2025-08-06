using TTT.API.Player;
using TTT.API.Role;

namespace TTT.API.Game;

public interface IGame : IDisposable {
  /// <summary>
  ///   The list of players in the game.
  ///   Spectators are not included in this list.
  /// </summary>
  ICollection<IPlayer> Players { get; }

  public IList<IRole> Roles { get; }

  IActionLogger Logger { get; }

  DateTime? StartedAt { get; }
  DateTime? FinishedAt { get; }
  IRole? WinningRole { get; set; }

  State State { get; set; }

  /// <summary>
  ///   Attempts to start a game.
  ///   Depending on implementation, this may start a countdown or immediately start the game.
  /// </summary>
  /// <param name="countdown"></param>
  IObservable<long>? Start(TimeSpan? countdown = null);

  void EndGame(EndReason? reason = null);

  bool IsInProgress() { return State is State.COUNTDOWN or State.IN_PROGRESS; }

  ISet<IOnlinePlayer> GetRole(Type roleType) {
    if (!typeof(IRole).IsAssignableFrom(roleType))
      throw new ArgumentException(
        "roleType must be a type that implements IRole", nameof(roleType));

    return Players.OfType<IOnlinePlayer>()
     .Where(p => p.Roles.Any(r => r.GetType().IsAssignableTo(roleType)))
     .ToHashSet();
  }

  ISet<IOnlinePlayer> GetAlive() {
    return Players.OfType<IOnlinePlayer>().Where(p => p.IsAlive).ToHashSet();
  }

  ISet<IOnlinePlayer> GetAlive(Type roleType) {
    if (!typeof(IRole).IsAssignableFrom(roleType))
      throw new ArgumentException(
        "roleType must be a type that implements IRole", nameof(roleType));

    return GetAlive()
     .Where(p => p.Roles.Any(r => r.GetType().IsAssignableTo(roleType)))
     .ToHashSet();
  }

  ISet<IOnlinePlayer> GetAlive(IRole role) { return GetAlive(role.GetType()); }
}