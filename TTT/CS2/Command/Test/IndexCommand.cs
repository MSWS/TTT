using CounterStrikeSharp.API;
using TTT.API.Command;
using TTT.API.Player;

namespace TTT.CS2.Command.Test;

public class IndexCommand : ICommand {
  public string Id => "index";

  public void Dispose() { }
  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);

    Server.NextWorldUpdate(() => {
      foreach (var player in Utilities.GetPlayers())
        info.ReplySync(
          $"{player.PlayerName} - {player.Slot} {player.Index} {player.DraftIndex} {player.PawnCharacterDefIndex}");
    });

    return Task.FromResult(CommandResult.SUCCESS);
  }
}