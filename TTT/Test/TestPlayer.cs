using TTT.API.Player;
using TTT.API.Role;

namespace TTT.Test;

public class TestPlayer(string id, string name) : IOnlinePlayer {
  public List<string> Messages { get; } = [];
  public string Id { get; } = id;
  public string Name { get; } = name;
  public ICollection<IRole> Roles { get; } = [];
  public int Health { get; set; } = 100;
  public int MaxHealth { get; set; } = 100;
  public int Armor { get; set; } = 100;
  public bool IsAlive { get; set; } = true;

  public static TestPlayer Random() {
    return new TestPlayer(new Random().NextInt64().ToString(),
      "Test Player " + Guid.NewGuid());
  }

  public override string ToString() { return $"TEST[{Id}|{Name}]"; }
}