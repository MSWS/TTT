using TTT.API.Player;

namespace TTT.API;

public interface IPermissionManager<in T> where T : IPlayer {
  bool HasFlags(T player, params string[] flags);
  bool InGroups(T player, params string[] groups);
}