using TTT.Api;
using TTT.Api.Player;

namespace TTT.Test;

public class TestPlayer(string id, string name) : IOnlinePlayer {
  public string Id { get; } = id;
  public string Name { get; } = name;
  public ICollection<IRole> Roles { get; } = (List<IRole>) [];

  public List<string> Messages { get; } = [];

  public TestPlayer() : this("314159", "Test Player") { }

  public static TestPlayer Random() {
    return new TestPlayer(new Random().NextInt64().ToString(),
      "Test Player " + Guid.NewGuid());
  }
}