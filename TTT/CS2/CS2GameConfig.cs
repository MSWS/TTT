using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using TTT.API;
using TTT.API.Storage;
using TTT.Game;

namespace TTT.CS2;

public class CS2GameConfig : IStorage<GameConfig>, IPluginModule {
  public static readonly FakeConVar<int> CV_TRAITOR_HEALTH =
    new FakeConVar<int>("css_ttt_rolehp_traitor",
      "Amount of health to give to traitors at start of round", 100,
      ConVarFlags.FCVAR_NONE, new RangeValidator<int>(1, 100));

  public Task<GameConfig> Load() {
    return Task.FromResult(new GameConfig {
      BalanceCfg = new GameConfig.BalanceConfig {
        TraitorHealth = CV_TRAITOR_HEALTH.Value
      }
    });
  }

  public void Dispose() { }
  public string Name => "CS2GameConfig";
  public string Version => GitVersionInformation.FullSemVer;

  public void Start() { }
}