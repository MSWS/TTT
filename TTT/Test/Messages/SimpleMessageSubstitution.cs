using JetBrains.Annotations;
using TTT.API.Events;
using TTT.Game.Events.Player;

namespace TTT.Test.Messages;

public partial class MessageModificationTest {
  private const string ORIGINAL_MESSAGE = "Original Message";
  private const string MODIFIED_MESSAGE = "Modified Message";

  private class SimpleMessageSubstitution(IEventBus bus) : IListener {
    public void Dispose() { bus.UnregisterListener(this); }

    [EventHandler]
    [UsedImplicitly]
    public void OnMessage(PlayerMessageEvent ev) {
      ev.Message = MODIFIED_MESSAGE;
    }
  }
}