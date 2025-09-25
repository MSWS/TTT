using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Command;
using TTT.API.Player;
using TTT.Game;
using TTT.Game.Commands;

namespace TTT.CS2.Command;

public class CS2CommandManager(IServiceProvider provider)
  : CommandManager(provider), IPluginModule {
  private const string COMMAND_PREFIX = "css_";

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private BasePlugin? plugin;

  public void Start(BasePlugin? basePlugin, bool hotReload) {
    plugin = basePlugin;
    base.Start();
  }

  public override bool RegisterCommand(ICommand command) {
    var registration = command.Aliases.All(alias
      => cmdMap.TryAdd(COMMAND_PREFIX + alias, command));
    if (!registration) return false;
    foreach (var alias in command.Aliases)
      plugin?.AddCommand(COMMAND_PREFIX + alias,
        command.Description ?? string.Empty, processInternal);
    return true;
  }

  private void
    processInternal(CCSPlayerController? executor, CommandInfo info) {
    var cs2Info = new CS2CommandInfo(Provider, info);
    var wrapper = executor == null ?
      null :
      converter.GetPlayer(executor) as IOnlinePlayer;
    Task.Run(async () => {
      try {
        Console.WriteLine($"Processing command: {cs2Info.CommandString}");
        return await ProcessCommand(cs2Info);
      } catch (Exception e) {
        var msg = e.Message;
        cs2Info.ReplySync(Localizer[GameMsgs.GENERIC_ERROR(msg)]);
        await Server.NextWorldUpdateAsync(() => {
          Console.WriteLine(
            $"Encountered an error when processing command: \"{cs2Info.CommandString}\" by {wrapper?.Id}");
          Console.WriteLine(e);
        });
        return CommandResult.ERROR;
      }
    });
  }
}