using TTT.Shop;

namespace ShopAPI.Configs;

public record OneShotDeagleConfig : ShopItemConfig {
  public override int Price { get; init; } = 100;
  public bool DoesFriendlyFire { get; init; } = true;
  public bool KillShooterOnFF { get; init; } = false;
  public string Weapon { get; init; } = "revolver";
  public int WeaponSlot { get; init; } = 1;
}
