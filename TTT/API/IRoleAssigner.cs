using TTT.API.Player;

namespace TTT.API;

public interface IRoleAssigner {
  public void AssignRoles(ISet<IOnlinePlayer> players, IList<IRole> roles);
}