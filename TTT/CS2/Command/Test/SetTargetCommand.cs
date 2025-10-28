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

    var name = "TRAITOR";

    if (info.ArgCount == 2) name = info.Args[1];

    Server.NextWorldUpdate(() => {
      var gamePlayer = converter.GetPlayer(executor);
      if (gamePlayer == null) return;

      var entity =
        Utilities.FindAllEntitiesByDesignerName<CLogicRelay>("logic_relay");

      var first =
        entity.FirstOrDefault(e => e.Globalname == "ttt_traitor_assigner");

      if (first == null) {
        info.ReplySync("Could not find logic_relay ttt_traitor_assigner");
      } else {
        first.AcceptInput("Trigger", gamePlayer, gamePlayer);
        info.ReplySync("Triggered logic_relay ttt_traitor_assigner");
      }

      gamePlayer.Pawn.Value?.AcceptInput("AddContext", null, null, "TRAITOR:1");

      if (gamePlayer.Entity != null) {
        info.ReplySync("Current entity name: " + gamePlayer.Entity.Name);
        EntityNameHelper.SetEntityName(gamePlayer.Entity, name);
        info.ReplySync("Set entity name to " + gamePlayer.Entity.Name);
      }

      info.ReplySync("Set target name to " + name);
    });
    return Task.FromResult(CommandResult.SUCCESS);
  }
}