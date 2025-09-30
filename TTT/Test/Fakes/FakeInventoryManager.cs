using TTT.API;
using TTT.API.Player;

namespace TTT.Test.Fakes;

public class FakeInventoryManager : IInventoryManager {
  public Task GiveWeapon(IOnlinePlayer player, IWeapon weapon) {
    throw new NotImplementedException();
  }

  public Task RemoveWeapon(IOnlinePlayer player, string weaponId) {
    throw new NotImplementedException();
  }

  public Task RemoveWeaponInSlot(IOnlinePlayer player, int slot) {
    throw new NotImplementedException();
  }

  public Task RemoveAllWeapons(IOnlinePlayer player) {
    throw new NotImplementedException();
  }
}