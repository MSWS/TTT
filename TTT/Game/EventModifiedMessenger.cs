using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace TTT.Game;

public abstract class EventModifiedMessenger(IServiceProvider provider)
  : IMessenger {
  protected readonly IEventBus Bus = provider.GetRequiredService<IEventBus>();

  protected readonly IPlayerFinder Players =
    provider.GetRequiredService<IPlayerFinder>();

  protected readonly IServiceProvider Provider = provider;

  public Task<bool> Message(IPlayer? player, string message,
    params object[] args) {
    if (player == null) return SendMessage(null, message, args);
    return sendMsg(player, message,
      new PlayerMessageEvent(player, message, args));
  }

  [Conditional("DEBUG")]
  public abstract void Debug(string msg, params object[] args);

  [Conditional("DEBUG")]
  public virtual void DebugAnnounce(string msg, params object[] args) {
    Debug(msg, args);
  }

  public Task<bool> MessageAll(string message, params object[] args) {
    throw new NotImplementedException();
  }

  public Task<bool> BackgroundMsgAll(string message, params object[] args) {
    throw new NotImplementedException();
  }

  public Task<bool> ScreenMsgAll(string message, params object[] args) {
    throw new NotImplementedException();
  }

  [Conditional("DEBUG")]
  public virtual void DebugInform(string msg, params object[] args) {
    Debug(msg, args);
  }

  // Allow for overriding in derived classes
  public virtual Task<bool> BackgroundMsg(IPlayer? player, string message,
    params object[] args) {
    return Message(player, message);
  }

  public virtual Task<bool> ScreenMsg(IPlayer? player, string message,
    params object[] args) {
    return Message(player, message);
  }

  private async Task<bool> sendMsg(IPlayer? player, string msg,
    PlayerMessageEvent ev) {
    if (player == null) return await SendMessage(null, msg);

    Bus.Dispatch(ev);
    if (ev.IsCanceled) return false;

    return await SendMessage(player, ev.Message, ev.Args);
  }

  public Task<bool> Message(IOnlinePlayer? player, string message,
    params object[] args) {
    return ((IMessenger)this).Message(player, message, args);
  }

  abstract protected Task<bool> SendMessage(IPlayer? player, string message,
    params object[] args);
}