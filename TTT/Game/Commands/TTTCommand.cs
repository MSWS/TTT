using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Command;
using TTT.API.Events;
using TTT.API.Player;
using TTT.Locale;

namespace TTT.Game.Commands;

public class TTTCommand(IServiceProvider provider) : ICommand {
  private readonly IMsgLocalizer localizer =
    provider.GetRequiredService<IMsgLocalizer>();

  public void Dispose() { }
  public string Name => "ttt";
  public string[] Usage => ["<modules/commands/listeners>"];

  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    var version =
      $"{GitVersionInformation.FullSemVer} ({GitVersionInformation.ShortSha})";
#if DEBUG
    version += "-DEBUG";
#endif
    if (info.ArgCount == 1) {
      info.ReplySync(localizer[GameMsgs.CMD_TTT(version)]);
      return Task.FromResult(CommandResult.SUCCESS);
    }

    var prefix = localizer[GameMsgs.PREFIX];

    if (info.ArgCount < 2) return Task.FromResult(CommandResult.SUCCESS);
    return Task.FromResult(handleSubcommand(info, prefix));
  }

  private CommandResult handleSubcommand(ICommandInfo info, string prefix) {
    switch (info.Args[1].ToLower()) {
      case "modules":
        var modules = provider.GetServices<ITerrorModule>()
         .Where(m => m is not IPluginModule);
        info.ReplySync(prefix + "Loaded Modules:");
        printModules(info, modules);

        info.ReplySync(prefix + "Loaded Plugin Modules:");
        var pluginModules = provider.GetServices<IPluginModule>();
        printModules(info, pluginModules);
        return CommandResult.SUCCESS;

      case "commands":
        var commands = provider.GetRequiredService<ICommandManager>().Commands;
        info.ReplySync(prefix + "Registered Commands:");
        printModules(info, commands);
        return CommandResult.SUCCESS;

      case "listeners":
        var listeners = provider.GetServices<IListener>();
        info.ReplySync(prefix + "Registered Listeners:");
        printModules(info, listeners);
        return CommandResult.SUCCESS;
      default:
        info.ReplySync(prefix + "Unknown specification.");
        return CommandResult.UNKNOWN_COMMAND;
    }
  }

  private void printModules(ICommandInfo info,
    IEnumerable<ITerrorModule> listeners) {
    foreach (var listener in listeners)
      printVersionedEntry(info, listener.Version,
        listener.Name + " - " + listener.GetType().Name);
  }

  private void printVersionedEntry(ICommandInfo info, string version,
    string value) {
    var color = getVersionColor(version);
    info.ReplySync($" {color}{version}{ChatColors.Grey}: {value}");
  }

  private char getVersionColor(string version) {
    var asciiSum = version.Sum(c => c);

    return (asciiSum % 10) switch {
      0 => ChatColors.Red,
      1 => ChatColors.Orange,
      2 => ChatColors.Yellow,
      3 => ChatColors.Green,
      4 => ChatColors.LightBlue,
      5 => ChatColors.Blue,
      6 => ChatColors.LightPurple,
      7 => ChatColors.Magenta,
      8 => ChatColors.Grey,
      _ => ChatColors.White
    };
  }
}