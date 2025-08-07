using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Game;
using TTT.API.Storage;
using TTT.Game;

namespace TTT.CS2.GameHandlers;

public class RoundStartHandler(IServiceProvider provider) : IPluginModule {
  public void Dispose() { throw new NotImplementedException(); }
  public string Name => nameof(RoundStartHandler);
  public string Version => GitVersionInformation.FullSemVer;

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  private readonly GameConfig config =
    provider.GetService<IStorage<GameConfig>>()?.Load().GetAwaiter().GetResult()
    ?? new GameConfig();

  public void Start() { }

  [GameEventHandler]
  public HookResult OnRoundStart(EventRoundStart _, GameEventInfo _1) {
    if (games.IsGameActive()) return HookResult.Continue;

    var game = games.CreateGame();
    game?.Start(config.RoundCfg.CountDownDuration);
    return HookResult.Continue;
  }
}