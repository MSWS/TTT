namespace TTT.API;

public interface ITerrorModule : IDisposable {
  string Id => GetType().Name;
  string Version => GitVersionInformation.FullSemVer;

  void Start();
}