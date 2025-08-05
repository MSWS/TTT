using JetBrains.Annotations;
using TTT.API.Events;
using TTT.Game.Events.Player;

namespace TTT.Test.Messages;

public partial class MessageModificationTest {
  private class SimpleArgsSubstitution(IEventBus bus) : IListener {
    public void Dispose() { bus.UnregisterListener(this); }

    [EventHandler]
    [UsedImplicitly]
    public void OnMessage(PlayerMessageEvent ev) { ev.Args = ["Modified Arg"]; }
  }
}