using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TTT.API;

namespace Plugin;

public class TTT(IServiceProvider provider) : BasePlugin {
  private IServiceScope scope = null!;
  public override string ModuleName => "TTT.Plugin";

  public override string ModuleVersion
    => $"{GitVersionInformation.BranchName}-{GitVersionInformation.FullSemVer}-{GitVersionInformation.BuildMetaData}";

  public override void Load(bool hotReload) {
    Logger.LogInformation($"{ModuleName} {ModuleVersion} Starting... ");

    scope = provider.CreateScope();
    var modules = scope.ServiceProvider.GetServices<ITerrorModule>().ToList();
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

    Logger.LogInformation("All modules loaded successfully.");
  }
}