using TTT.Api.Player;

namespace TTT.Api;

public interface IRoleAssigner {
  public void AssignRoles(ISet<IOnlinePlayer> players, IList<IRole> roles);
}