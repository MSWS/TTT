using TTT.API.Role;

namespace TTT.API.Player;

public interface IOnlinePlayer : IPlayer {
  ICollection<IRole> Roles { get; }
  public int Health { get; set; }
  public int MaxHealth { get; set; }
  public int Armor { get; set; }
  public bool IsAlive { get; set; }

  void GiveWeapon(string weaponId);
  void RemoveWeapon(string weaponId);
  void RemoveAllWeapons();
}