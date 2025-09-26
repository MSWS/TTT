using TTT.API;

namespace TTT.Test.Abstract;

public class PluginInitTester : IPluginModule {
  public int Starts { get; private set; }
  public void Dispose() { }

  public void Start() { Starts++; }
}