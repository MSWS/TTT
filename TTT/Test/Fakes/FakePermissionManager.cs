using System.Security;
using TTT.API.Player;

namespace TTT.Test.Fakes;

public class FakePermissionManager : IPermissionManager {
  public bool HasFlags(IPlayer player, params string[] flags) { return true; }

  public bool InGroups(IPlayer player, params string[] groups) { return true; }
}