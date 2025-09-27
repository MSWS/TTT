using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Player;
using TTT.CS2.Hats;

namespace TTT.CS2.Command.Test;

public class ScreenTextCommand(IServiceProvider provider) : ICommand {
  public string Name => "screentext";

  private readonly ITextSpawner spawner =
    provider.GetRequiredService<ITextSpawner>();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  public void Dispose() { }
  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    var textSetting = new TextSetting { msg = "Foo" };

    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);

    Server.NextWorldUpdate(() => {
      var ents =
        Utilities.FindAllEntitiesByDesignerName<CPointWorldText>(
          "point_worldtext");
      foreach (var ent in ents) ent.AcceptInput("Kill");
      var player = converter.GetPlayer(executor);
      if (player == null || !player.IsValid) return;
      spawner.CreateTextScreen(textSetting, player);
      info.ReplySync("Spawned screen text.");
    });

    return Task.FromResult(CommandResult.SUCCESS);
  }
}