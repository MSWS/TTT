namespace ShopAPI.Configs.Traitor;

public record GlovesConfig : ShopItemConfig {
  public override int Price { get; init; } = 65;
  public int MaxUses { get; init; } = 3;
}