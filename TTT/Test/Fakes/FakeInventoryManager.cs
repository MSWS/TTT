using TTT.API;
using TTT.API.Player;

namespace TTT.Test.Fakes;

public class FakeInventoryManager : IInventoryManager {
  public Task GiveWeapon(IOnlinePlayer player, IWeapon weapon) {
    return Task.CompletedTask;
  }

  public Task RemoveWeapon(IOnlinePlayer player, string weaponId) {
    return Task.CompletedTask;
  }

  public Task RemoveWeaponInSlot(IOnlinePlayer player, int slot) {
    return Task.CompletedTask;
  }

  public Task RemoveAllWeapons(IOnlinePlayer player) {
    return Task.CompletedTask;
  }
}