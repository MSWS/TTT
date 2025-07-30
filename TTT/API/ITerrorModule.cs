namespace TTT.Api;

public interface ITerrorModule : IDisposable {
  string Name { get; }
  string Version { get; }

  void Start();
}