namespace ShopAPI.Configs.Traitor;

public record OneHitKnifeConfig : ShopItemConfig {
  public override int Price { get; init; } = 70;
  public bool FriendlyFire { get; init; } = true;
}