using TTT.Api;
using TTT.Api.Player;

namespace TTT.Test;

public class TestPlayer(string id, string name) : IOnlinePlayer {
  public string Id { get; } = id;
  public string Name { get; } = name;
  public ICollection<IRole> Roles { get; } = (List<IRole>) [];
}