using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using ShopAPI.Configs.Detective;
using TTT.API;
using TTT.API.Storage;

namespace TTT.CS2.Configs.ShopItems;

public class CS2DnaScannerConfig : IStorage<DnaScannerConfig>, IPluginModule {
  public static readonly FakeConVar<int> CV_PRICE = new(
    "css_ttt_shop_dna_price", "Price of the DNA Scanner item (Detective)", 110,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 10000));

  public static readonly FakeConVar<int> CV_MAX_SAMPLES = new(
    "css_ttt_shop_dna_max_samples",
    "Maximum number of DNA samples that can be stored at once", 0,
    ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 100));

  public static readonly FakeConVar<int> CV_DECAY_TIME_SECONDS = new(
    "css_ttt_shop_dna_decay_time",
    "Time (in seconds) before a DNA sample decays", 120, ConVarFlags.FCVAR_NONE,
    new RangeValidator<int>(10, 3600));

  public void Dispose() { }

  public void Start() { }

  public void Start(BasePlugin? plugin) {
    ArgumentNullException.ThrowIfNull(plugin, nameof(plugin));
    plugin.RegisterFakeConVars(this);
  }

  public Task<DnaScannerConfig?> Load() {
    var cfg = new DnaScannerConfig {
      Price      = CV_PRICE.Value,
      MaxSamples = CV_MAX_SAMPLES.Value,
      DecayTime  = TimeSpan.FromSeconds(CV_DECAY_TIME_SECONDS.Value)
    };

    return Task.FromResult<DnaScannerConfig?>(cfg);
  }
}