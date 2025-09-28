using TTT.API.Player;
using TTT.API.Role;

namespace TTT.Test;

public class TestPlayer(string id, string name) : IOnlinePlayer {
  public List<string> Messages { get; } = [];

  [Obsolete(
    "Roles are now managed via IRoleAssigner. Use IRoleAssigner.GetRoles(IPlayer) instead.")]
  public ICollection<IRole> Roles { get; } = [];

  public string Id { get; } = id;
  public string Name { get; } = name;

  public int Health { get; set; } = 100;
  public int MaxHealth { get; set; } = 100;
  public int Armor { get; set; } = 100;

  public bool IsAlive {
    get => Health > 0;
    set {
      if (!value)
        Health = 0;
      else if (Health <= 0) { Health = 1; }
    }
  }

  public static TestPlayer Random() {
    return new TestPlayer(new Random().NextInt64().ToString(),
      "Test Player " + Guid.NewGuid());
  }

  public override string ToString() { return $"TEST[{Id}|{Name}]"; }
}