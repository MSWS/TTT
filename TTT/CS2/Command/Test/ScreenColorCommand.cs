using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;
using TTT.CS2.Extensions;

namespace TTT.CS2.Command.Test;

public class ScreenColorCommand(IServiceProvider provider) : ICommand {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  public string Name => "screencolor";

  public void Dispose() { }
  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);
    Server.NextWorldUpdate(() => {
      var   player = converter.GetPlayer(executor);
      float hold   = 0.5f, fade = 0.5f;
      if (info.ArgCount >= 2) float.TryParse(info.Args[1], out hold);
      if (info.ArgCount >= 3) float.TryParse(info.Args[2], out fade);

      player?.ColorScreen(Color.Red, hold, fade);

      info.ReplySync("Colored your screen red.");
    });
    return Task.FromResult(CommandResult.SUCCESS);
  }
}