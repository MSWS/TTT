using TTT.API.Player;

namespace TTT.Test.Fakes;

public class FakeInventoryManager : IInventoryManager {
  public void GiveWeapon(IOnlinePlayer player, string weaponId) { }

  public void RemoveWeapon(IOnlinePlayer player, string weaponId) { }

  public void RemoveAllWeapons(IOnlinePlayer player) { }
}