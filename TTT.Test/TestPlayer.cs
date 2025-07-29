using TTT.Api;
using TTT.Api.Player;

namespace TTT.Test;

public class TestPlayer(string id, string name) : IOnlinePlayer {
  private readonly List<string> weapons = [];
  public TestPlayer() : this("314159", "Test Player") { }

  public List<string> Messages { get; } = [];
  public string Id { get; } = id;
  public string Name { get; } = name;
  public ICollection<IRole> Roles { get; } = (List<IRole>) [];
  public int Health { get; set; }
  public int MaxHealth { get; set; }
  public int Armor { get; set; }
  public bool IsAlive { get; set; }

  public void GiveWeapon(string weaponId) { weapons.Add(weaponId); }

  public void RemoveWeapon(string weaponId) { weapons.Remove(weaponId); }

  public void RemoveAllWeapons() { weapons.Clear(); }

  public static TestPlayer Random() {
    return new TestPlayer(new Random().NextInt64().ToString(),
      "Test Player " + Guid.NewGuid());
  }

  public override string ToString() { return $"TEST[{id}|{name}]"; }
}