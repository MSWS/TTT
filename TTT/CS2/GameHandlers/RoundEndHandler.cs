using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;
using TTT.Game.Roles;

namespace TTT.CS2.GameHandlers;

public class RoundEndHandler(IServiceProvider provider) : IPluginModule {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  public void Dispose() { }

  public string Name => nameof(RoundEndHandler);
  public string Version => GitVersionInformation.FullSemVer;

  public void Start() { }

  [GameEventHandler]
  public HookResult OnRoundEnd(EventRoundEnd ev, GameEventInfo _) {
    if (!games.IsGameActive()) return HookResult.Continue;
    var game = games.ActiveGame ?? throw new InvalidOperationException(
      "Active game is null, but round end event was triggered.");
    game.EndGame(EndReason.TIMEOUT(new InnocentRole(provider)));
    return HookResult.Continue;
  }
}