using TTT.API;

namespace TTT.Test.Abstract;

public class GenericInitTester : ITerrorModule {
  public int Starts { get; private set; }
  public void Dispose() { }
  public string Name => nameof(GenericInitTester);
  public string Version => GitVersionInformation.FullSemVer;

  public void Start() { Starts++; }
}