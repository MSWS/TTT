using CounterStrikeSharp.API.Core;
using GitVersion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TTT.Api;

namespace TTT.Core;

public class TTT(IServiceProvider provider) : BasePlugin {
  public override string ModuleName => "TTT.Plugin";

  public override string ModuleVersion
    => $"{GitVersionInformation.BranchName}-{GitVersionInformation.FullSemVer}-{GitVersionInformation.BuildMetaDataPadded}";

  private IServiceScope scope = null!;

  public override void Load(bool hotReload) {
    Logger.LogInformation($"{ModuleName} {ModuleVersion} Starting...");

    scope = provider.CreateScope();
    var modules = scope.ServiceProvider.GetServices<IPluginModule>().ToList();

    Logger.LogInformation($"Found {modules.Count} modules to load.");

    foreach (var module in modules) {
      RegisterAllAttributes(module);
      module.Start(this, hotReload);
      Logger.LogInformation($"Loaded {module.Name} ({module.Version})");
    }

    Logger.LogInformation("All modules loaded successfully.");
  }
}