namespace TTT.API.Player;

public interface IInventoryManager {
  /// <summary>
  ///   Gives a weapon to the player.
  /// </summary>
  /// <param name="player">The player to give the weapon to.</param>
  /// <param name="weaponId">The ID of the weapon to give.</param>
  void GiveWeapon(IOnlinePlayer player, string weaponId);

  /// <summary>
  ///   Removes a weapon from the player.
  /// </summary>
  /// <param name="player">The player to remove the weapon from.</param>
  /// <param name="weaponId">The ID of the weapon to remove.</param>
  void RemoveWeapon(IOnlinePlayer player, string weaponId);

  /// <summary>
  ///   Removes all weapons from the player.
  /// </summary>
  /// <param name="player">The player to remove all weapons from.</param>
  void RemoveAllWeapons(IOnlinePlayer player);
}