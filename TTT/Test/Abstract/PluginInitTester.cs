using TTT.API;

namespace TTT.Test.Abstract;

public class PluginInitTester : IPluginModule {
  public void Dispose() { }
  public string Name => nameof(PluginInitTester);
  public string Version => GitVersionInformation.FullSemVer;

  public int Starts { get; private set; }

  public void Start() { Starts++; }
}