using TTT.API;

namespace ShopAPI.Configs.Traitor;

public record ClusterGrenadeConfig : ShopItemConfig, IWeapon {
  public override int Price { get; init; } = 100;
  public int GrenadeCount { get; init; } = 8;
  public string WeaponId { get; } = "weapon_hegrenade";
  public int? ReserveAmmo { get; } = null;
  public int? CurrentAmmo { get; } = null;
  public float UpForce { get; init; } = 200f;
  public float ThrowForce { get; init; } = 300f;
}