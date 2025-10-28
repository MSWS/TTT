using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;
using TTT.CS2.Utils;

namespace TTT.CS2.Command.Test;

public class SetTargetCommand(IServiceProvider provider) : ICommand {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  public void Dispose() { }
  public void Start() { }

  public string Id => "settarget";

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);

    Server.NextWorldUpdate(() => {
      var gamePlayer = converter.GetPlayer(executor);
      if (gamePlayer == null) return;

      if (gamePlayer.Entity != null) {
        // gamePlayer.Entity.Name = "TRAITOR";
        // Utilities.SetStateChanged(gamePlayer, "CEntityIdentity", "m_name");

        EntityNameHelper.SetEntityName(gamePlayer.Entity, "TRAITOR");

        info.ReplySync("Set entity name to " + gamePlayer.Entity.Name);
      }

      info.ReplySync("Set target name to TRAITOR");
    });
    return Task.FromResult(CommandResult.SUCCESS);
  }
}