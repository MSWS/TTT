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
    if (activator.IsValid) {
      messenger.Debug("Activator: " + activator.DesignerName);
      activator.AcceptInput("OnPass");
    }

    if (caller.IsValid) {
      messenger.Debug("Caller: " + caller.DesignerName);
      caller.AcceptInput("OnPass");
    }

    if (value.IsValid)
      messenger.Debug("Value: " + value + " " + value.GetType());
    if (!caller.IsValid || !caller.DesignerName.StartsWith("filter_"))
      return HookResult.Continue;
    var csPlayer = Utilities.GetPlayerFromIndex((int)activator.Index);
    if (csPlayer != null && csPlayer.IsValid) {
      messenger.DebugAnnounce(
        $"{caller.DesignerName} triggered by player: {csPlayer.PlayerName} {(int)csPlayer.Index}");
    }

    messenger.DebugAnnounce(output + " - " + output.Description);

    var connections = output.Connections;
    if (connections != null) debugConnection(connections);

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