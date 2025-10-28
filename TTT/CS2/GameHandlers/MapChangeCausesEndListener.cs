using CounterStrikeSharp.API;
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
    plugin?.RegisterListener<CounterStrikeSharp.API.Core.Listeners.OnMapStart>(
      onMapChange);
  }

  private void onMapChange(string mapName) {
    games.ActiveGame?.EndGame(new EndReason("Map Change"));
    Server.PrintToConsole("Detected map change, ending active game.");
  }
}