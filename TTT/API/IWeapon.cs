namespace TTT.API;

public interface IWeapon {
  /// <summary>
  ///   The internal ID of the weapon, should match the ID of the weapon in the underlying game.
  /// </summary>
  public string Id { get; }

  /// <summary>
  ///   The amount of ammo that is in reserve for this weapon.
  /// </summary>
  public int? ReserveAmmo { get; }

  /// <summary>
  ///   The amount of ammo that is currently in the weapon's magazine or chamber.
  /// </summary>
  public int? CurrentAmmo { get; }
}