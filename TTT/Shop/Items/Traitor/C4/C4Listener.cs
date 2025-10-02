using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using TTT.API;

namespace TTT.Shop.Items.Traitor.C4;

public class C4Listener : IPluginModule {
  [GameEventHandler(HookMode.Pre)]
  public HookResult OnBombExplode(EventBombExploded ev, GameEventInfo info) {
    Server.PrintToChatAll("Bomb has exploded!");
    return HookResult.Handled;
  }

  [GameEventHandler(HookMode.Pre)]
  public HookResult
    OnBombBeginPlant(EventBombBeginplant ev, GameEventInfo info) {
    Server.PrintToChatAll("Bomb has begun planting!");
    return HookResult.Continue;
  }

  [GameEventHandler(HookMode.Pre)]
  public HookResult OnBombPlanted(EventBombPlanted ev, GameEventInfo info) {
    Server.PrintToChatAll("Bomb was planted!");
    return HookResult.Handled;
  }

  public void Dispose() { }
  public void Start() { }
}