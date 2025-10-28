using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using JetBrains.Annotations;
using TTT.API;

namespace TTT.CS2.Items.TeleportDecoy;

public class TeleportDecoyListener : IPluginModule {
  public void Dispose() { }
  public void Start() { }

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnDecoyDetonate(EventDecoyDetonate ev, GameEventInfo _) {
    Server.PrintToChatAll(
      $"Decoy detonated at position {ev.X}, {ev.Y}, {ev.Z}");
    return HookResult.Continue;
  }
}