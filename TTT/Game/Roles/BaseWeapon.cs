using TTT.API;

namespace TTT.Game.Roles;

public class BaseWeapon(string id, int? reserve = null,
  int? current = null) : IWeapon {
  public string Id { get; } = id;
  public int? ReserveAmmo { get; } = reserve;
  public int? CurrentAmmo { get; } = current;
}