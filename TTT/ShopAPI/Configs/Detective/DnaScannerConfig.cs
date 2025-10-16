namespace ShopAPI.Configs.Detective;

public record DnaScannerConfig : ShopItemConfig {
  public override int Price { get; init; } = 110;
  public int MaxSamples { get; init; } = 0;
  public TimeSpan DecayTime { get; init; } = TimeSpan.FromMinutes(2);
}