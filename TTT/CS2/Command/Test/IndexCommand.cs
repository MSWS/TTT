using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;

namespace TTT.CS2.Command.Test;

public class IndexCommand(IServiceProvider provider) : ICommand {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  public string Id => "index";

  public void Dispose() { }
  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);

    Server.NextWorldUpdate(() => {
      foreach (var player in Utilities.GetPlayers())
        info.ReplySync($"{player.PlayerName} - {player.Slot}");
    });

    return Task.FromResult(CommandResult.SUCCESS);
  }
}