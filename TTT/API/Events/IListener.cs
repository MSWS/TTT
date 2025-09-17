namespace TTT.API.Events;

public interface IListener : IDisposable, ITerrorModule {
  void ITerrorModule.Start() { }
}