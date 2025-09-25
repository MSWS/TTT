using System.Reactive.Concurrency;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Locale;

namespace TTT.Game.Listeners;

public abstract class BaseListener(IServiceProvider provider) : IListener {
  protected readonly IEventBus Bus = provider.GetRequiredService<IEventBus>();

  protected readonly IPlayerFinder Finder =
    provider.GetRequiredService<IPlayerFinder>();

  protected readonly IGameManager Games =
    provider.GetRequiredService<IGameManager>();

  protected readonly IMsgLocalizer Locale =
    provider.GetRequiredService<IMsgLocalizer>();

  protected readonly IMessenger Messenger =
    provider.GetRequiredService<IMessenger>();

  protected readonly IServiceProvider Provider = provider;

  protected readonly IRoleAssigner Roles =
    provider.GetRequiredService<IRoleAssigner>();

  protected readonly IScheduler Scheduler =
    provider.GetRequiredService<IScheduler>();

  public virtual void Dispose() { Bus.UnregisterListener(this); }

  public virtual void Start() { }
}