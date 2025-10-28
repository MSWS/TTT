using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Messages;

namespace TTT.CS2.GameHandlers;

public class EntityTargetHandlers(IServiceProvider provider) : IPluginModule {
  private readonly IMessenger messenger =
    provider.GetRequiredService<IMessenger>();

  public void Dispose() { }
  public void Start() { }

  public void Start(BasePlugin? plugin) {
    plugin?.HookEntityOutput("*", "*", handler);
  }

  private HookResult handler(CEntityIOOutput output, string name,
    CEntityInstance activator, CEntityInstance caller, CVariant value,
    float delay) {
    messenger.Debug("Entity Output Triggered: " + name);
    messenger.Debug("Activator: " + activator.DesignerName);
    messenger.Debug("Caller: " + caller.DesignerName);
    messenger.Debug("Value: " + value);
    if (caller.DesignerName == "prop_dynamic") return HookResult.Continue;
    if (caller.DesignerName != "filter_activator_name")
      return HookResult.Continue;
    var csPlayer = Utilities.GetPlayerFromIndex((int)activator.Index);
    if (csPlayer == null || !csPlayer.IsValid) return HookResult.Continue;
    messenger.DebugAnnounce("Filter Activator Name triggered by player: "
      + csPlayer.PlayerName);
    messenger.DebugAnnounce(output + " - " + output.Description);
    return HookResult.Continue;
  }
}