using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;

namespace TTT.CS2.Command.Test;

public class EmitSoundCommand(IServiceProvider provider) : ICommand {
  public string Id => "emitsound";

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  public void Dispose() { }
  public void Start() { }

  public string[] Usage => ["[sound]"];

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);

    var gamePlayer = converter.GetPlayer(executor);
    if (gamePlayer == null) return Task.FromResult(CommandResult.PLAYER_ONLY);
    if (info.Args.Length < 2) return Task.FromResult(CommandResult.PRINT_USAGE);
    Server.NextWorldUpdate(() => {
      gamePlayer.EmitSound(info.Args[1]);
      info.ReplySync("Emitted sound " + info.Args[1]);
    });
    return Task.FromResult(CommandResult.SUCCESS);
  }
}