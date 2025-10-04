using CounterStrikeSharp.API.Core;
using TTT.API;

namespace TTT.CS2.GameHandlers;

public class BombPlantSuppressor : IPluginModule {
  public void Dispose() { }

  public void Start() { }

  public void Start(BasePlugin? plugin) {
    plugin?.HookUserMessage(322, um => {
      um.Recipients.Clear();
      return HookResult.Handled;
    });
  }
}