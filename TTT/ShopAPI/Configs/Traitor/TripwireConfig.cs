using System.Drawing;

namespace ShopAPI.Configs.Traitor;

public record TripwireConfig : ShopItemConfig {
  public override int Price { get; init; } = 60;
  public int ExplosionPower { get; init; } = 1000;
  public float FalloffDelay { get; init; } = 0.02f;
  public float FriendlyFireMultiplier { get; init; } = 0.5f;
  public bool FriendlyFireTriggers { get; init; } = true;
  public float MaxPlacementDistanceSquared { get; init; } = 400f * 400f;

  public TimeSpan TripwireInitiationTime { get; init; } =
    TimeSpan.FromSeconds(2);

  public float TripwireSizeSquared { get; init; } = 500f;
  public Color TripwireColor { get; init; } = Color.FromArgb(64, Color.Red);
  public float TripwireThickness { get; init; } = 0.5f;

  public TimeSpan DefuseTime { get; init; } = TimeSpan.FromSeconds(5);
  public TimeSpan DefuseRate { get; init; } = TimeSpan.FromMilliseconds(500);

  public int DefuseReward { get; init; } = 20;
}