using TTT.API;

namespace TTT.Test.Abstract;

public class GenericInitTester : ITerrorModule {
  public void Dispose() { }
  public string Name => nameof(GenericInitTester);
  public string Version => GitVersionInformation.FullSemVer;

  public int Starts { get; private set; }

  public void Start() { Starts++; }
}