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

  public void Start() { }

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    var version =
      $"{GitVersionInformation.FullSemVer} ({GitVersionInformation.ShortSha})";
#if DEBUG
    version += "-DEBUG";
#endif
    info.ReplySync(localizer[GameMsgs.CMD_TTT(version)]);

    var prefix = localizer[GameMsgs.PREFIX];

    if (info.ArgCount < 2) return Task.FromResult(CommandResult.SUCCESS);

    switch (info.Args[1].ToLower()) {
      case "modules":
        var modules = provider.GetServices<ITerrorModule>()
         .Where(m => m is not IPluginModule);
        info.ReplySync(prefix + "Loaded Modules:");
        foreach (var module in modules)
          printVersionedEntry(info, module.Version,
            module.Name + " - " + module.GetType().Name);

        info.ReplySync(prefix + "Loaded Plugin Modules:");
        var pluginModules = provider.GetServices<IPluginModule>();
        foreach (var module in pluginModules)
          printVersionedEntry(info, module.Version,
            module.Name + " - " + module.GetType().Name);

        break;
      case "commands":
        var commands = provider.GetRequiredService<ICommandManager>().Commands;
        info.ReplySync(prefix + "Registered Commands:");
        foreach (var command in commands)
          printVersionedEntry(info, command.Version,
            command.Name + " - " + command.GetType().Name);

        break;

      case "listeners":
        var listeners = provider.GetServices<IListener>();
        info.ReplySync(prefix + "Registered Listeners:");
        foreach (var listener in listeners)
          printVersionedEntry(info, listener.Version,
            listener.Name + " - " + listener.GetType().Name);

        break;
    }

    return Task.FromResult(CommandResult.SUCCESS);
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