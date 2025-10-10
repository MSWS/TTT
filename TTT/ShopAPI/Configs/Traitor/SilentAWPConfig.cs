using TTT.API;

namespace ShopAPI.Configs.Traitor;

public record SilentAWPConfig : ShopItemConfig, IWeapon {
  public override int Price { get; init; }
  public string WeaponId { get; } = "weapon_awp";
  public int? ReserveAmmo { get; } = 0;
  public int? CurrentAmmo { get; } = 2;
  public int WeaponIndex { get; } = 9;
}