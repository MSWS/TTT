using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using Xunit;

namespace TTT.Test.Abstract;

public class ModuleInitializationTest(IServiceProvider provider) {
  private readonly GenericInitTester tester =
    provider.GetRequiredService<GenericInitTester>();

  [Fact]
  public void Started_ShouldNotBeInit_IfNotStarted() {
    Assert.Equal(0, tester.Starts);
  }

  [Fact]
  public void Started_ShouldBeInit_IfStarted() {
    var modules = provider.GetServices<ITerrorModule>()
     .Where(p => p is not IPluginModule)
     .ToList();

    Assert.Equal(1, modules.Count);

    foreach (var module in modules) module.Start();

    Assert.Equal(1, tester.Starts);
  }

  [Fact]
  public void PluginStarted_ShouldBeInit_IfStarted() {
    var pluginTester = provider.GetRequiredService<PluginInitTester>();

    Assert.Equal(0, pluginTester.Starts);

    var modules = provider.GetServices<IPluginModule>().ToList();

    Assert.Equal(1, modules.Count);

    foreach (var module in modules) module.Start();

    Assert.Equal(1, pluginTester.Starts);
  }
}