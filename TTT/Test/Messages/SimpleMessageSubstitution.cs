using JetBrains.Annotations;
using TTT.API.Events;
using TTT.Game.Events.Player;

namespace TTT.Test.Messages;

public partial class MessageModificationTest {
  private const string ORIGINAL_MESSAGE = "Original Message";
  private const string MODIFIED_MESSAGE = "Modified Message";

  private class SimpleMessageSubstitution<T>(IEventBus bus)
    : IListener where T : PlayerMessageEvent {
    public void Dispose() { bus.UnregisterListener(this); }

    [EventHandler]
    [UsedImplicitly]
    public void OnMessage(T ev) { ev.Message = MODIFIED_MESSAGE; }
  }
}