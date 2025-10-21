using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Command;
using TTT.API.Player;

namespace TTT.CS2.Command.Test;

public class ReloadModuleCommand(IServiceProvider provider) : ICommand, IPluginModule {
  public void Dispose() { }
  public void Start() { }
  private BasePlugin? plugin;

  public string Id => "reload";

  public void Start(BasePlugin? plugin) {
    if (plugin == null) return;
    this.plugin = plugin;
  }

  public string[] Usage => ["<module>"];

  public Task<CommandResult>
    Execute(IOnlinePlayer? executor, ICommandInfo info) {
    if (info.ArgCount != 2) return Task.FromResult(CommandResult.INVALID_ARGS);

    var moduleName = info.Args[1];
    var modules    = provider.GetServices<ITerrorModule>();

    var module = modules.FirstOrDefault(m
      => m.Id.Equals(moduleName, StringComparison.OrdinalIgnoreCase));
    if (module == null) {
      info.ReplySync($"Module '{moduleName}' not found.");
      return Task.FromResult(CommandResult.INVALID_ARGS);
    }

    info.ReplySync("Reloading module '{moduleName}'...");
    module.Dispose();

    info.ReplySync("Starting module '{moduleName}'...");
    module.Start();
    info.ReplySync("Module '{moduleName}' reloaded successfully.");

    if (plugin == null) {
      info.ReplySync("Plugin context not found; skipping hotload steps.");
      return Task.FromResult(CommandResult.SUCCESS);
    }

    if (module is not IPluginModule pluginModule)
      return Task.FromResult(CommandResult.SUCCESS);

    Server.NextWorldUpdate(() => {
      info.ReplySync($"Hotloading plugin module '{moduleName}'...");
      pluginModule.Start(plugin, true);
      info.ReplySync($"Plugin module '{moduleName}' hotloaded successfully.");
    });

    return Task.FromResult(CommandResult.SUCCESS);
  }
}