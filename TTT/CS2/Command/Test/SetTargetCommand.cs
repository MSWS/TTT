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


  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);

    Server.NextWorldUpdate(() => {
      var csPlayer = converter.GetPlayer(executor);
      if (csPlayer == null) return;
      EntityNameHelper.SetEntityName(csPlayer, EntityNameHelper.Role.Traitor);
      info.ReplySync($"Set entity name for player {executor.Name}");
    });
    return Task.FromResult(CommandResult.SUCCESS);
  }
}