using TTT.API;

namespace ShopAPI.Configs.Traitor;

public record ClusterGrenadeConfig : ShopItemConfig, IWeapon {
  public override int Price { get; init; } = 80;
  public int GrenadeCount { get; init; } = 8;
  public string WeaponId { get; } = "weapon_hegrenade";
  public int? ReserveAmmo { get; } = null;
  public int? CurrentAmmo { get; } = null;
}