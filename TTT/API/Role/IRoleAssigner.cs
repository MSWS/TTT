using TTT.API.Player;
using TTT.API.Storage;

namespace TTT.API.Role;

public interface IRoleAssigner : IKeyedStorage<IPlayer, ICollection<IRole>>,
  IKeyWritable<IPlayer, ICollection<IRole>> {
  /// <summary>
  ///   Will attempt to assign roles to all players in the set.
  ///   Note that game-specific behavior and logic will almost certainly
  ///   change how this behaves.
  ///   Role assigning will call <see cref="IRole.FindPlayerToAssign" /> on the
  ///   first role continuously until it returns null, then it will move on to
  ///   second role, and so on.
  ///   Thus, the order of roles **does matter**. If you want a role to be assigned
  ///   first, it should be the first in the list.
  /// </summary>
  /// <param name="players"></param>
  /// <param name="roles"></param>
  public void AssignRoles(ISet<IOnlinePlayer> players, IList<IRole> roles);

  public void SetRole(IOnlinePlayer player, IRole role) {
    Write(player, new List<IRole> { role }).GetAwaiter().GetResult();
  }

  public ICollection<IRole> GetRoles(IPlayer player) {
    return Load(player).GetAwaiter().GetResult() ?? [];
  }
}