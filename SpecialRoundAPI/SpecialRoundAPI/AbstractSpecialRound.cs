using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using SpecialRound;
using TTT.API;
using TTT.API.Events;
using TTT.Game.Events.Game;
using TTT.Locale;

namespace SpecialRoundAPI;

public abstract class AbstractSpecialRound(IServiceProvider provider)
  : ITerrorModule, IListener {
  protected readonly IServiceProvider Provider = provider;

  protected readonly ISpecialRoundTracker Tracker =
    provider.GetRequiredService<ISpecialRoundTracker>();

  public void Dispose() { }
  public void Start() { }

  public abstract IMsg Description { get; }
  public abstract SpecialRoundConfig Config { get; }

  public abstract void ApplyRoundEffects();

  [UsedImplicitly]
  [EventHandler]
  public abstract void OnGameState(GameStateUpdateEvent ev);
}