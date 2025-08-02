using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TTT.API;
using TTT.API.Command;

namespace TTT.CS2;

public class CS2CommandManager(IServiceProvider provider,
  ICommandManager commandManager) : IPluginModule {
  private bool hotReload;
  private BasePlugin? plugin;

  public void Start(BasePlugin? basePlugin, bool baseReload) {
    plugin    = basePlugin;
    hotReload = baseReload;

    //Add Commands Here
    registerCommand(new TTTCommand(provider));

    foreach (var command in provider.GetServices<ICommand>()) {
      command.Start();
      registerCommand(command);
    }
  }

  public void Dispose() { }
  public string Name => "CommandManager";
  public string Version => GitVersionInformation.FullSemVer;
  public void Start() { }

  private bool registerCommand(ICommand command) {
    if (!commandManager.RegisterCommand(command)) return false;

    foreach (var alias in command.Aliases)
      plugin?.AddCommand(alias, command.Description ?? string.Empty,
        processInternal);

    return true;
  }

  private void
    processInternal(CCSPlayerController? executor, CommandInfo info) {
    var player      = executor is null ? null : new CS2Player(executor);
    var wrappedInfo = new CS2CommandInfo(info);

    Task.Run(async () => {
      try {
        await commandManager.ProcessCommand(player, wrappedInfo);
      } catch (Exception ex) {
        var logger = provider.GetRequiredService<ILoggerFactory>()
         .CreateLogger("CommandManager");
        await Server.NextFrameAsync(() => logger.LogError(ex,
          "Error running command \"{command}\" by {steam}",
          wrappedInfo.GetCommandString,
          executor?.SteamID.ToString() ?? "Console"));

        wrappedInfo.ReplySync(string.IsNullOrEmpty(ex.Message) ?
          "An unexpected error occurred." :
          $"Error: {ex.Message}");
      }
    });
  }
}