namespace TTT.API.Player;

public interface IPermissionManager {
  bool HasFlags(IPlayer player, params string[] flags);
  bool InGroups(IPlayer player, params string[] groups);
}