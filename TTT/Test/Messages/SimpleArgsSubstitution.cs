using JetBrains.Annotations;
using TTT.API.Events;
using TTT.Game.Events.Player;

namespace TTT.Test.Messages;

public partial class MessageModificationTest {
  private class SimpleArgsSubstitution<T>(IEventBus bus)
    : IListener where T : PlayerMessageEvent {
    public void Dispose() { bus.UnregisterListener(this); }

    [EventHandler]
    [UsedImplicitly]
    public void OnMessage(T ev) { ev.Args = ["Modified Arg"]; }
  }
}