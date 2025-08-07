using System.Reactive.Linq;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;

namespace TTT.Plugin;

public class TTT(IServiceProvider provider) : BasePlugin {
  private IServiceScope scope = null!;
  public override string ModuleName => "TTT.Plugin";

  public override string ModuleVersion
    => $"{GitVersionInformation.BranchName}-{GitVersionInformation.FullSemVer}-{GitVersionInformation.BuildMetaData}";

  public override void Load(bool hotReload) {
    Logger.LogInformation($"{ModuleName} {ModuleVersion} Starting... ");

    scope = provider.CreateScope();
    var modules = scope.ServiceProvider.GetServices<ITerrorModule>()
     .Where(m => m is not IPluginModule)
     .ToList();
    Logger.LogInformation($"Found {modules.Count} base modules to load.");

    foreach (var module in modules) {
      module.Start();
      Logger.LogInformation($"Loaded {module.Name} ({module.Version})");
    }

    var pluginModules =
      scope.ServiceProvider.GetServices<IPluginModule>().ToList();

    Logger.LogInformation(
      $"Found {pluginModules.Count} plugin modules to load.");

    foreach (var module in pluginModules) {
      RegisterAllAttributes(module);
      module.Start(this, hotReload);
      Logger.LogInformation($"Loaded {module.Name} ({module.Version})");
    }

    var listeners = scope.ServiceProvider.GetServices<IListener>().ToList();
    var eventBus  = scope.ServiceProvider.GetRequiredService<IEventBus>();
    Logger.LogInformation($"Found {listeners.Count} listeners to load.");
    foreach (var listener in listeners) {
      eventBus.RegisterListener(listener);
      Logger.LogInformation($"Registered listener {listener.GetType()}");
    }

    Logger.LogInformation("All modules loaded successfully.");
  }

  override protected void Dispose(bool disposing) {
    if (!disposing) {
      base.Dispose(disposing);
      return;
    }

    provider.GetService<IGameManager>()?.Dispose();

    base.Dispose(disposing);
  }
}