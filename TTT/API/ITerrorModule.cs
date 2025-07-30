namespace TTT.API;

public interface ITerrorModule : IDisposable {
  string Name { get; }
  string Version { get; }

  void Start();
}