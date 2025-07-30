using TTT.API.Player;
using TTT.API.Role;

namespace TTT.Test;

public class TestPlayer(string id, string name) : IOnlinePlayer {
  private readonly List<string> weapons = [];
  private readonly HashSet<string> flags = [ "@ttt/test" ];
  private readonly HashSet<string> groups = [ "#ttt/test" ];
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
  public bool HasFlags(params string[] flags) { throw new NotImplementedException(); }
  public bool InGroups(params string[] groups) { throw new NotImplementedException(); }

  public static TestPlayer Random() {
    return new TestPlayer(new Random().NextInt64().ToString(),
      "Test Player " + Guid.NewGuid());
  }

  public override string ToString() { return $"TEST[{Id}|{Name}]"; }
}