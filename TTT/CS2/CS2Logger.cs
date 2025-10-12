using CounterStrikeSharp.API;
using Serilog;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Game.Loggers;

namespace TTT.CS2;

public class CS2Logger(IServiceProvider provider) : SimpleLogger(provider) {
  public override void PrintLogs() {
    Server.NextWorldUpdate(() => base.PrintLogs());
  }

  public override void PrintLogs(IOnlinePlayer? player) {
    Server.NextWorldUpdate(() => base.PrintLogs(player));
  }

  public override void LogAction(IAction action) {
    Server.NextWorldUpdate(() => base.LogAction(action));
  }
}