using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;

namespace TTT.CS2.Command.Test;

public class SpecCommand(IServiceProvider provider) : ICommand {
  public void Dispose() { }
  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    var target = executor;

    if (info.ArgCount == 2) {
      var finder = provider.GetRequiredService<IPlayerFinder>();
      var result = finder.GetPlayerByName(info.Args[1]);
      if (result == null) {
        info.ReplySync($"Player '{info.Args[1]}' not found.");
        return Task.FromResult(CommandResult.ERROR);
      }

      target = result;
    } else if (target == null) {
      return Task.FromResult(CommandResult.PLAYER_ONLY);
    }

    var converter =
      provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

    Server.NextWorldUpdate(() => {
      var player = converter.GetPlayer(target);
      player?.ChangeTeam(CsTeam.Spectator);
      info.ReplySync($"{target.Name} has been moved to Spectators.");
    });
    return Task.FromResult(CommandResult.SUCCESS);
  }
}