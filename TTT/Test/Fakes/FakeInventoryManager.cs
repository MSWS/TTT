using TTT.API;
using TTT.API.Player;

namespace TTT.Test.Fakes;

public class FakeInventoryManager : IInventoryManager {
  public void GiveWeapon(IOnlinePlayer player, IWeapon weapon) { }
  public void RemoveWeapon(IOnlinePlayer player, string weaponId) { }
  public void RemoveWeaponInSlot(IOnlinePlayer player, int slot) { }
  public void RemoveAllWeapons(IOnlinePlayer player) { }
}