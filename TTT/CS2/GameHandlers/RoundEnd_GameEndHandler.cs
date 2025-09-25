using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Game;
using TTT.Game.Roles;

namespace TTT.CS2.GameHandlers;

public class RoundEnd_GameEndHandler(IServiceProvider provider) : IPluginModule {
  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  public void Dispose() { }

  public void Start() { }

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnRoundEnd(EventRoundEnd _, GameEventInfo _1) {
    if (games.ActiveGame is not { State: State.IN_PROGRESS })
      return HookResult.Continue;
    var game = games.ActiveGame ?? throw new InvalidOperationException(
      "Active game is null, but round end event was triggered.");
    if (game.FinishedAt != null)
      // We caused this round to end already, don't end it again
      return HookResult.Continue;

    game.EndGame(EndReason.TIMEOUT(new InnocentRole(provider)));
    return HookResult.Continue;
  }
}