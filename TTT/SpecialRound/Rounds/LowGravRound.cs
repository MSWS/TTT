using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Cvars;
using Microsoft.Extensions.DependencyInjection;
using SpecialRound.lang;
using SpecialRoundAPI;
using SpecialRoundAPI.Configs;
using TTT.API.Game;
using TTT.API.Storage;
using TTT.Game.Events.Game;
using TTT.Locale;

namespace SpecialRound.Rounds;

public class LowGravRound(IServiceProvider provider)
  : AbstractSpecialRound(provider) {
  private int originalGravity = 800;
  public override string Name => "Low Grav";
  public override IMsg Description => RoundMsgs.SPECIAL_ROUND_LOWGRAV;
  public override SpecialRoundConfig Config => config;

  private LowGravRoundConfig config
    => Provider.GetService<IStorage<LowGravRoundConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new LowGravRoundConfig();

  public override void ApplyRoundEffects() {
    var cvar = ConVar.Find("sv_gravity");
    if (cvar == null) return;

    originalGravity = cvar.GetPrimitiveValue<int>();
    var newGravity = (int)(originalGravity * config.GravityMultiplier);
    Server.NextWorldUpdate(()
      => Server.ExecuteCommand($"sv_gravity {newGravity}"));
  }

  public override void OnGameState(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;

    Server.NextWorldUpdate(()
      => Server.ExecuteCommand($"sv_gravity {originalGravity}"));
  }
}