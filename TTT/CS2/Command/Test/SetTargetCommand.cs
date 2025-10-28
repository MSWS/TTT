using System.Numerics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;
using TTT.CS2.Extensions;
using TTT.CS2.Utils;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace TTT.CS2.Command.Test;

public class SetTargetCommand(IServiceProvider provider) : ICommand {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  public void Dispose() { }
  public void Start() { }

  public string Id => "settarget";

  private static readonly Vector RELAY_POSITION = new(69, 420, -60);

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);

    var name = "TRAITOR";

    if (info.ArgCount == 2) name = info.Args[1];

    Server.NextWorldUpdate(() => {
      var gamePlayer = converter.GetPlayer(executor);
      if (gamePlayer == null) return;

      var entity = Utilities
       .FindAllEntitiesByDesignerName<CLogicRelay>("logic_relay")
       .FirstOrDefault(e
          => e.Entity?.Name == "ttt_traitor_assigner"
          && e.AbsOrigin.DistanceSquared(RELAY_POSITION) < 100);

      if (entity == null) {
        info.ReplySync("Could not find logic_relay ttt_traitor_assigner");
      } else {
        entity.AcceptInput("Trigger", gamePlayer, gamePlayer);
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