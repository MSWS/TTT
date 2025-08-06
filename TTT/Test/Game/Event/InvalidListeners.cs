using TTT.API.Events;

namespace TTT.Test.Game.Event;

public static class InvalidListeners {
  public class NoMethodListener : IListener {
    public void Dispose() { }
  }

  public class NoParamListener : IListener {
    public void Dispose() { }

    [EventHandler]
    public void OnEvent() {
      // No parameters, should throw
    }
  }

  public class WrongParamListener : IListener {
    public void Dispose() { }

    [EventHandler]
    public void OnEvent(string message) {
      // Invalid parameter type, should throw
    }
  }
}