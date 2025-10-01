using TTT.Shop;

namespace ShopAPI.Configs;

public record DnaScannerConfig : ShopItemConfig {
  public override int Price { get; init; } = 100;
  public int MaxSamples { get; init; } = 0;
  public TimeSpan DecayTime { get; init; } = TimeSpan.FromMinutes(3);
}