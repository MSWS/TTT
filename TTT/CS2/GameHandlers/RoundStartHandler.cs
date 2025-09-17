using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Game;
using TTT.API.Storage;
using TTT.Game;

namespace TTT.CS2.GameHandlers;

public class RoundStartHandler(IServiceProvider provider) : IPluginModule {
  private readonly TTTConfig config =
    provider.GetService<IStorage<TTTConfig>>()?.Load().GetAwaiter().GetResult()
    ?? new TTTConfig();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  public void Dispose() { throw new NotImplementedException(); }

  public void Start() { }

  [GameEventHandler]
  public HookResult OnRoundStart(EventRoundStart _, GameEventInfo _1) {
    if (games.IsGameActive()) return HookResult.Continue;

    var game = games.CreateGame();
    game?.Start(config.RoundCfg.CountDownDuration);
    return HookResult.Continue;
  }
}