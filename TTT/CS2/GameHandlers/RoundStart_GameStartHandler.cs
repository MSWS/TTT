using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.Game;

namespace TTT.CS2.GameHandlers;

public class RoundStart_GameStartHandler(IServiceProvider provider)
  : IPluginModule {
  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  private TTTConfig config
    => provider.GetService<IStorage<TTTConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new TTTConfig();

  public void Dispose() { }

  public void Start() { }

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnRoundStart(EventRoundStart _, GameEventInfo _1) {
    if (games.ActiveGame is { State: State.IN_PROGRESS or State.COUNTDOWN })
      return HookResult.Continue;

    var count = finder.GetOnline().Count;
    if (count < config.RoundCfg.MinimumPlayers) return HookResult.Continue;

    var game = games.CreateGame();
    game?.Start(config.RoundCfg.CountDownDuration);
    return HookResult.Continue;
  }

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnWarmupEnd(EventWarmupEnd ev, GameEventInfo _1) {
    if (games.ActiveGame is { State: State.IN_PROGRESS or State.COUNTDOWN })
      return HookResult.Continue;

    var count = finder.GetOnline().Count;
    if (count < config.RoundCfg.MinimumPlayers) return HookResult.Continue;

    var game = games.CreateGame();
    game?.Start(config.RoundCfg.CountDownDuration);
    return HookResult.Continue;
  }
}