namespace TTT.API;

public interface ITerrorModule : IDisposable {
  string Name => GetType().Name;
  string Version => GitVersionInformation.FullSemVer;

  void Start();
}