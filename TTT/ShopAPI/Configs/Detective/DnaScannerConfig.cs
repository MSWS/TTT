namespace ShopAPI.Configs.Detective;

public record DnaScannerConfig : ShopItemConfig {
  public override int Price { get; init; } = 100;
  public int MaxSamples { get; init; } = 0;
  public TimeSpan DecayTime { get; init; } = TimeSpan.FromSeconds(10);
}