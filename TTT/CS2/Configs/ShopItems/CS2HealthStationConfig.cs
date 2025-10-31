using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using ShopAPI.Configs.Detective;
using TTT.API;
using TTT.API.Storage;

namespace TTT.CS2.Configs.ShopItems;

public class CS2HealthStationConfig : IStorage<HealthStationConfig>,
  IPluginModule {
  public static readonly FakeConVar<int> CV_PRICE = new(
    "css_ttt_shop_healthstation_price",
    "Price of the Health Station item (Detective)", 50, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(0, 10000));

  public static readonly FakeConVar<string> CV_USE_SOUND = new(
    "css_ttt_shop_healthstation_use_sound",
    "Sound played when using the Health Station", "sounds/buttons/blip1");

  public static readonly FakeConVar<int> CV_HEALTH_INCREMENTS = new(
    "css_ttt_shop_healthstation_increments",
    "Number of health increments applied per use", 10, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(1, 100));

  public static readonly FakeConVar<int> CV_HEALTH_INTERVAL = new(
    "css_ttt_shop_healthstation_interval",
    "Interval (in seconds) between health increments", 1,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(1, 60));

  public static readonly FakeConVar<int> CV_STATION_HEALTH = new(
    "css_ttt_shop_healthstation_station_health",
    "Maximum health of the station object itself", 200, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(50, 1000));

  public static readonly FakeConVar<int> CV_TOTAL_HEALTH_GIVEN = new(
    "css_ttt_shop_healthstation_total_health_given",
    "Total health the station can provide before depleting (0 = infinite)", 0,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 10000));

  public static readonly FakeConVar<float> CV_MAX_RANGE = new(
    "css_ttt_shop_healthstation_max_range",
    "Maximum range (in units) from which players can use the station", 256f,
    ConVarFlags.FCVAR_NONE, new RangeValidator<float>(50f, 2048f));

  public void Dispose() { }

  public void Start() { }

  public void Start(BasePlugin? plugin) {
    ArgumentNullException.ThrowIfNull(plugin, nameof(plugin));
    plugin.RegisterFakeConVars(this);
  }

  public Task<HealthStationConfig?> Load() {
    var cfg = new HealthStationConfig {
      Price            = CV_PRICE.Value,
      UseSound         = CV_USE_SOUND.Value,
      HealthIncrements = CV_HEALTH_INCREMENTS.Value,
      HealthInterval   = TimeSpan.FromSeconds(CV_HEALTH_INTERVAL.Value),
      StationHealth    = CV_STATION_HEALTH.Value,
      TotalHealthGiven = CV_TOTAL_HEALTH_GIVEN.Value,
      MaxRange         = CV_MAX_RANGE.Value
    };

    return Task.FromResult<HealthStationConfig?>(cfg);
  }
}