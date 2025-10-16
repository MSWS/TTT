using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.UserMessages;
using TTT.API;

namespace TTT.CS2.GameHandlers;

public class DebugMessage : IPluginModule {
  public void Dispose() { }
  public void Start() { }

  public void Start(BasePlugin? plugin) {
    for (var i = 0; i < 10000; i++) {
      if (i is 325 or 124) continue;
      var j = i;
      plugin.HookUserMessage(i, msg => debug(msg, j));
    }
  }

  private HookResult debug(UserMessage native, int id) {
    Server.PrintToConsole(id + "");
    Server.PrintToConsole(native.DebugString);
    return HookResult.Continue;
  }
}