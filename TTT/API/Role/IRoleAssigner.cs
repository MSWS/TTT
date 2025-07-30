using TTT.API.Player;

namespace TTT.API.Role;

public interface IRoleAssigner {
  public void AssignRoles(ISet<IOnlinePlayer> players, IList<IRole> roles);
}