using TTT.API;

namespace TTT.CS2.GameHandlers;

public class PropMover : IPluginModule {
  public void Dispose() { throw new NotImplementedException(); }

  public string Name => nameof(PropMover);
  public string Version => GitVersionInformation.FullSemVer;
  public void Start() { throw new NotImplementedException(); }
}