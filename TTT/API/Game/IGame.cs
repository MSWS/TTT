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

  IRoleAssigner RoleAssigner { get; init; }

  /// <summary>
  ///   Attempts to start a game.
  ///   Depending on implementation, this may start a countdown or immediately start the game.
  /// </summary>
  /// <param name="countdown">TimeSpan for countdown, null means start immediately</param>
  IObservable<long>? Start(TimeSpan? countdown = null);

  void EndGame(EndReason? reason = null);

  bool CheckEndConditions();

  ISet<IOnlinePlayer> GetAlive() {
    return Players.OfType<IOnlinePlayer>().Where(p => p.IsAlive).ToHashSet();
  }

  ISet<IOnlinePlayer> GetAlive(Type roleType, bool loose = true) {
    if (!typeof(IRole).IsAssignableFrom(roleType))
      throw new ArgumentException(
        "roleType must be a type that implements IRole", nameof(roleType));

    if (!loose)
      return GetAlive()
       .Where(p => RoleAssigner.GetRoles(p).Any(r => r.GetType() == roleType))
       .ToHashSet();

    return GetAlive()
     .Where(p => RoleAssigner.GetRoles(p).Any(roleType.IsInstanceOfType))
     .ToHashSet();
  }

  ISet<IOnlinePlayer> GetAlive(IRole role, bool loose = true) {
    return GetAlive(role.GetType(), loose);
  }
}