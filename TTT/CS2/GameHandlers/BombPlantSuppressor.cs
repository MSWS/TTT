using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using JetBrains.Annotations;
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