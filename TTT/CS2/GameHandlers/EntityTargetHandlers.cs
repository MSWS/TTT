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
    if (caller.DesignerName == "prop_dynamic") return HookResult.Continue;
    messenger.Debug("Entity Output Triggered: " + name);
    messenger.Debug("Activator: " + activator.DesignerName);
    messenger.Debug("Caller: " + caller.DesignerName);
    messenger.Debug("Value: " + value + " " + value.GetType());
    caller.AcceptInput("OnPass");
    activator.AcceptInput("OnPass");
    if (caller.DesignerName != "filter_activator_name")
      return HookResult.Continue;
    var csPlayer =
      Utilities.GetPlayerFromIndex((int)activator.EntityHandle.Index);
    if (csPlayer != null && csPlayer.IsValid) {
      messenger.DebugAnnounce(
        $"Filter Activator Name triggered by player: {csPlayer.PlayerName} {(int)csPlayer.Index}");
    }

    var ptrPlayer = new CCSPlayerController(activator.Handle);
    if (ptrPlayer.IsValid) {
      messenger.DebugAnnounce(
        $"Filter Activator Name triggered by player controller: {ptrPlayer.PlayerName} {(int)ptrPlayer.Index}");
    }

    messenger.DebugAnnounce(output + " - " + output.Description);

    var connections = output.Connections;
    if (connections != null) debugConnection(connections);

    caller.AcceptInput("OnPass");
    return HookResult.Continue;
  }

  private void debugConnection(EntityIOConnection_t connection) {
    messenger.DebugAnnounce("Connection:");
    messenger.DebugAnnounce("  Target: " + connection.Target);
    messenger.DebugAnnounce("  Input: " + connection.TargetInput);
    messenger.DebugAnnounce("  Parameter: " + connection.ValueOverride);
    messenger.DebugAnnounce("  Delay: " + connection.Delay);
    messenger.DebugAnnounce("  Times to fire: " + connection.TimesToFire);

    if (connection.Next != null) debugConnection(connection.Next);
  }
}