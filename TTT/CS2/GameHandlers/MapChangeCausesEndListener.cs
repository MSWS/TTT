using CounterStrikeSharp.API.Core;
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
    plugin?.RegisterListener<CounterStrikeSharp.API.Core.Listeners.OnMapStart>(
      onMapChange);
  }

  private void onMapChange(string mapName) {
    if (games.ActiveGame is not { State: State.IN_PROGRESS or State.COUNTDOWN })
      return;
    games.ActiveGame?.EndGame(new EndReason("Map Change"));
  }
}