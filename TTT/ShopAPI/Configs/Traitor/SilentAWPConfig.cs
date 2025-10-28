using TTT.API;

namespace ShopAPI.Configs.Traitor;

public record SilentAWPConfig : ShopItemConfig, IWeapon {
  public override int Price { get; init; } = 80;
  public int WeaponIndex { get; init; } = 9;
  public string WeaponId { get; init; } = "weapon_awp";
  public int? ReserveAmmo { get; init; } = 0;
  public int? CurrentAmmo { get; init; } = 1;
}