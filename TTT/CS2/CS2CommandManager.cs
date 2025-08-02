using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using TTT.API;
using TTT.API.Command;
using TTT.API.Player;
using TTT.Game.Commands;

namespace TTT.CS2;

public class CS2CommandManager(IServiceProvider provider)
  : CommandManager(provider), IPluginModule {
  private bool hotReload;
  private BasePlugin? plugin;

  private const string COMMAND_PREFIX = "css_";

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  public void Start(BasePlugin? basePlugin, bool baseReload) {
    plugin    = basePlugin;
    hotReload = baseReload;

    RegisterCommand(new TTTCommand(Provider));

    foreach (var command in Provider.GetServices<ICommand>()) command.Start();
  }

  public override bool RegisterCommand(ICommand command) {
    command.Start();
    var registration = command.Aliases.All(alias
      => Commands.TryAdd(COMMAND_PREFIX + alias, command));
    if (registration == false) return false;
    foreach (var alias in command.Aliases)
      plugin?.AddCommand(COMMAND_PREFIX + alias,
        command.Description ?? string.Empty, processInternal);
    return true;
  }

  private void
    processInternal(CCSPlayerController? executor, CommandInfo info) {
    var cs2Info = new CS2CommandInfo(info);
    var wrapper = executor == null ?
      null :
      converter.GetPlayer(executor) as IOnlinePlayer;
    Task.Run(async () => {
      try {
        Console.WriteLine($"Processing command: {cs2Info.GetCommandString}");
        return await ProcessCommand(wrapper, cs2Info);
      } catch (Exception e) {
        var msg = e.Message;
        // await Server.NextFrameAsync(() => {
        //   provider.GetRequiredService<ILoggerFactory>()
        //    .CreateLogger("Gangs")
        //    .LogError(e,
        //       "Encountered an error when processing command: \"{command}\" by {steam}",
        //       wrappedInfo.GetCommandString, wrapper?.Steam);
        // });
        // wrappedInfo.ReplySync(string.IsNullOrEmpty(msg) ?
        //   Locale.Get(MSG.GENERIC_ERROR) :
        //   Locale.Get(MSG.GENERIC_ERROR_INFO, msg));
        return CommandResult.ERROR;
      }
    });
  }

  public void Dispose() { }
  public string Name => "CommandManager";
  public string Version => GitVersionInformation.FullSemVer;
  public void Start() { }
}