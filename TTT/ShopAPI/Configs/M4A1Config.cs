namespace ShopAPI.Configs;

public record M4A1Config : ShopItemConfig {
  public override int Price { get; init; } = 85;
  public int[] ClearSlots { get; init; } = [0, 1];
  public string[] Weapons { get; init; } = ["m4a1", "usps"];
}