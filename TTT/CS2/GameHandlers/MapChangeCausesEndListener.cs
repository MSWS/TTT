using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Game;

namespace TTT.CS2.GameHandlers;

public class MapChangeCausesEndListener(IServiceProvider provider)
  : IPluginModule {
  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  public void Dispose() { }
  public void Start() { }
  public void Start(BasePlugin? plugin) {
  }

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnMapChange(EventMapTransition ev, GameEventInfo _) {
    games.ActiveGame?.EndGame(new EndReason("Map Change"));
    return HookResult.Continue;
  }
}