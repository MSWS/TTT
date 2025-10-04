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

  public abstract void Debug(string msg, params object[] args);

  public virtual void DebugAnnounce(string msg, params object[] args) {
    Debug(msg, args);
  }

  public virtual void DebugInform(string msg, params object[] args) {
    Debug(msg, args);
  }

  public Task<bool> MessageAll(string message, params object[] args) {
    return forAll(p => Message(p, message, args));
  }

  public Task<bool> BackgroundMsgAll(string message, params object[] args) {
    return forAll(p => BackgroundMsg(p, message, args));
  }

  public Task<bool> ScreenMsgAll(string message, params object[] args) {
    return forAll(p => ScreenMsg(p, message, args));
  }

  // Allow for overriding in derived classes
  public virtual Task<bool> BackgroundMsg(IPlayer? player, string message,
    params object[] args) {
    if (player == null) return SendMessage(null, message, args);
    return sendMsg(player, message,
      new PlayerBackgroundMessageEvent(player, message, args));
  }

  public virtual Task<bool> ScreenMsg(IPlayer? player, string message,
    params object[] args) {
    if (player == null) return SendMessage(null, message, args);
    return sendMsg(player, message,
      new PlayerScreenMessageEvent(player, message, args));
  }

  private async Task<bool> forAll(Func<IOnlinePlayer?, Task<bool>> action) {
    var players = Players.GetOnline();
    if (players.Count == 0) return true;
    var tasks = new List<Task<bool>>(players.Count);
    tasks.AddRange(players.Select(action));
    tasks.Add(action(null));
    var results = await Task.WhenAll(tasks);
    return results.All(r => r);
  }

  private async Task<bool> sendMsg(IPlayer? player, string msg,
    PlayerMessageEvent ev) {
    if (player == null) return await SendMessage(null, msg);

    await Bus.Dispatch(ev);
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