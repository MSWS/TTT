using CounterStrikeSharp.API;
using Microsoft.Extensions.DependencyInjection;
using SpecialRound.lang;
using SpecialRoundAPI;
using SpecialRoundAPI.Configs;
using TTT.API.Game;
using TTT.API.Storage;
using TTT.Game.Events.Game;
using TTT.Locale;

namespace SpecialRound.Rounds;

public class BhopRound(IServiceProvider provider)
  : AbstractSpecialRound(provider) {
  public override string Name => "BHop";
  public override IMsg Description => RoundMsgs.SPECIAL_ROUND_BHOP;
  public override SpecialRoundConfig Config => config;

  private BhopRoundConfig config
    => Provider.GetService<IStorage<BhopRoundConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new BhopRoundConfig();

  public override void ApplyRoundEffects() {
    Server.NextWorldUpdate(() => {
      Server.ExecuteCommand("sv_enablebunnyhopping 1");
      Server.ExecuteCommand("sv_autobunnyhopping 1");
    });
  }

  public override void OnGameState(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;

    Server.NextWorldUpdate(() => {
      Server.ExecuteCommand("sv_enablebunnyhopping 0");
      Server.ExecuteCommand("sv_autobunnyhopping 0");
    });
  }
}