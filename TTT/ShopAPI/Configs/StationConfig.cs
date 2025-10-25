using System.Drawing;

namespace ShopAPI.Configs;

public abstract record StationConfig : ShopItemConfig {
  public virtual int HealthIncrements { get; init; } = 5;
  public virtual int TotalHealthGiven { get; init; } = 0;
  public virtual int StationHealth { get; init; } = 200;
  public virtual float MaxRange { get; init; } = 256;

  public virtual TimeSpan HealthInterval { get; init; } =
    TimeSpan.FromSeconds(1);

  public abstract string UseSound { get; init; }
  public abstract Color GetColor(float health);
}