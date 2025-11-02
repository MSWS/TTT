using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using SpecialRoundAPI.Configs;
using TTT.API;
using TTT.API.Events;
using TTT.Game.Events.Game;
using TTT.Game.Listeners;
using TTT.Locale;

namespace SpecialRoundAPI;

public abstract class AbstractSpecialRound(IServiceProvider provider)
  : BaseListener(provider) {
  protected readonly ISpecialRoundTracker Tracker =
    provider.GetRequiredService<ISpecialRoundTracker>();

  public abstract string Name { get; }
  public abstract IMsg Description { get; }
  public abstract SpecialRoundConfig Config { get; }

  public abstract void ApplyRoundEffects();

  [UsedImplicitly]
  [EventHandler]
  public abstract void OnGameState(GameStateUpdateEvent ev);
}