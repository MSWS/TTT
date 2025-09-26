using CounterStrikeSharp.API;
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
}