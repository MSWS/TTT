using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.API.Player;

namespace TTT.Game.Listeners;

public abstract class BaseListener(IServiceProvider provider)
  : IListener, ITerrorModule {
  protected readonly IEventBus Bus = provider.GetRequiredService<IEventBus>();

  protected readonly IGameManager Games =
    provider.GetRequiredService<IGameManager>();

  protected readonly IPlayerFinder Finder =
    provider.GetRequiredService<IPlayerFinder>();

  protected readonly IOnlineMessenger Messenger =
    provider.GetRequiredService<IOnlineMessenger>();
  
  protected readonly IServiceProvider Provider = provider;

  public virtual void Dispose() { Bus.UnregisterListener(this); }
  public abstract string Name { get; }
  public string Version => GitVersionInformation.FullSemVer;

  public virtual void Start() { Bus.RegisterListener(this); }
}