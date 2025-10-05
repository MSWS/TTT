using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using JetBrains.Annotations;
using TTT.API;

namespace TTT.CS2.GameHandlers;

public class MapZoneRemover : IPluginModule {
  private BasePlugin? plugin;

  public void Dispose() {
    plugin?.RemoveListener<CounterStrikeSharp.API.Core.Listeners.OnMapStart>(
      onMapStart);
  }

  public void Start() { }

  private bool zonesRemoved = false;

  public void Start(BasePlugin? pluginParent) {
    if (pluginParent != null) this.plugin = pluginParent;
    plugin?.RegisterListener<CounterStrikeSharp.API.Core.Listeners.OnMapStart>(
      onMapStart);
  }

  private void onMapStart(string mapName) { zonesRemoved = false; }

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnRoundStart(EventRoundStart _, GameEventInfo _2) {
    if (zonesRemoved) return HookResult.Continue;
    var buyzones =
      Utilities.FindAllEntitiesByDesignerName<CBuyZone>("func_buyzone");
    foreach (var zone in buyzones) zone.Remove();

    zonesRemoved = true;
    return HookResult.Continue;
  }
}