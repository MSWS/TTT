using TTT.Api.Events;
using TTT.Api.Messages;
using TTT.Api.Player;
using TTT.Game.Events.Player;

namespace TTT.Game;

public abstract class EventModifiedMessenger(IEventBus bus) : IMessenger {
  public Task<bool> Message(IPlayer player, string message) {
    var messageEvent = new PlayerMessageEvent(player, message);
    bus.Dispatch(messageEvent);
    return messageEvent.IsCanceled ?
      Task.FromResult(false) :
      SendMessage(player, messageEvent.Message);
  }

  // Allow for overriding in derived classes
  public virtual Task<bool> BackgroundMsg(IPlayer player, string message) {
    return Message(player, message);
  }

  public virtual Task<bool> ScreenMsg(IPlayer player, string message) {
    return Message(player, message);
  }

  abstract protected Task<bool> SendMessage(IPlayer player, string message);
}