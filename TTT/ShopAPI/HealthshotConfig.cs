using ShopAPI.Configs;

namespace ShopAPI;

public record HealthshotConfig : ShopItemConfig {
  public override int Price { get; init; } = 30;
  public string Weapon { get; init; } = "weapon_healthshot";
}