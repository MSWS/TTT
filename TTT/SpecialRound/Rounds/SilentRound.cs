using Microsoft.Extensions.DependencyInjection;
using SpecialRound.lang;
using SpecialRoundAPI;
using SpecialRoundAPI.Configs;
using TTT.API.Storage;
using TTT.Game.Events.Game;
using TTT.Locale;

namespace SpecialRound.Rounds;

public class SilentRound(IServiceProvider provider)
  : AbstractSpecialRound(provider) {
  public override string Name => "Silent";
  public override IMsg Description => RoundMsgs.SPECIAL_ROUND_SILENT;
  public override SpecialRoundConfig Config => config;

  private SilentRoundConfig config
    => Provider.GetService<IStorage<SilentRoundConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new SilentRoundConfig();

  public override void ApplyRoundEffects() { }

  public override void OnGameState(GameStateUpdateEvent ev) { }
}