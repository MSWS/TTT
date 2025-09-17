using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TTT.API;
using TTT.API.Command;
using TTT.API.Events;
using TTT.API.Game;

namespace TTT.Plugin;

public class TTT(IServiceProvider provider) : BasePlugin {
  private IServiceScope scope = null!;
  public override string ModuleName => "TTT";

  public override string ModuleVersion
    => $"{GitVersionInformation.BranchName}-{GitVersionInformation.FullSemVer}-{GitVersionInformation.BuildMetaData}";

  private readonly List<ITerrorModule> loadedModules = [];

  public override void Load(bool hotReload) {
    Logger.LogInformation($"{ModuleName} {ModuleVersion} Starting... ");

    scope = provider.CreateScope();
    var modules = scope.ServiceProvider.GetServices<ITerrorModule>().ToList();
    Logger.LogInformation($"Found {modules.Count} base modules to load.");

    foreach (var module in modules) {
      module.Start();
      Logger.LogInformation(
        $"Loaded {module.Version} {module.Name} {module.GetType().Namespace}");
      loadedModules.Add(module);
    }

    var pluginModules =
      scope.ServiceProvider.GetServices<IPluginModule>().ToList();

    Logger.LogInformation(
      $"Found {pluginModules.Count} plugin modules, registering attributes...");

    foreach (var module in pluginModules) RegisterAllAttributes(module);

    Logger.LogInformation("All modules loaded successfully.");
  }

  override protected void Dispose(bool disposing) {
    if (!disposing) {
      base.Dispose(disposing);
      return;
    }

    foreach (var module in loadedModules) {
      try {
        Logger.LogInformation($"Unloading {module.Name} ({module.Version})");
        module.Dispose();
      } catch (Exception e) {
        Logger.LogError(e, $"Error unloading module {module.Name}");
      }
    }

    base.Dispose(disposing);
  }
}