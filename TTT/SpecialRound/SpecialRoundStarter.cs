using CounterStrikeSharp.API.Core;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using SpecialRound.Events;
using SpecialRound.lang;
using SpecialRoundAPI;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Storage;
using TTT.Game.Events.Game;
using TTT.Game.Listeners;

namespace SpecialRound;

public class SpecialRoundStarter(IServiceProvider provider)
  : BaseListener(provider), IPluginModule, ISpecialRoundStarter {
  private readonly ISpecialRoundTracker tracker =
    provider.GetRequiredService<ISpecialRoundTracker>();

  private int roundsSinceMapChange;

  private SpecialRoundsConfig config
    => Provider.GetService<IStorage<SpecialRoundsConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new SpecialRoundsConfig();

  public void Start(BasePlugin? plugin) {
    plugin?.RegisterListener<Listeners.OnMapStart>(onMapChange);
  }

  public AbstractSpecialRound?
    TryStartSpecialRound(AbstractSpecialRound? round) {
    round ??= getSpecialRound();

    var ev = new SpecialRoundStartEvent(round);
    Provider.GetService<IEventBus>()?.Dispatch(ev);

    if (ev.IsCanceled) return null;

    Messenger.MessageAll(Locale[RoundMsgs.SPECIAL_ROUND_STARTED(round)]);
    Messenger.MessageAll(Locale[round.Description]);

    round.ApplyRoundEffects();
    tracker.CurrentRound           = round;
    tracker.RoundsSinceLastSpecial = 0;
    return round;
  }

  private void onMapChange(string mapName) { roundsSinceMapChange = 0; }

  [UsedImplicitly]
  [EventHandler]
  public void OnRound(GameStateUpdateEvent ev) {
    if (ev.NewState != State.IN_PROGRESS) return;

    roundsSinceMapChange++;
    tracker.RoundsSinceLastSpecial++;

    if (tracker.RoundsSinceLastSpecial < config.MinRoundsBetweenSpecial) return;

    if (ev.Game.Players.Count < config.MinPlayersForSpecial) return;
    if (roundsSinceMapChange < config.MinRoundsAfterMapChange) return;
    if (Random.Shared.NextSingle() > config.SpecialRoundChance) return;

    var specialRound = getSpecialRound();

    TryStartSpecialRound(specialRound);
  }

  private AbstractSpecialRound getSpecialRound() {
    var rounds = Provider.GetServices<ITerrorModule>()
     .OfType<AbstractSpecialRound>()
     .Where(r => r.Config.Weight > 0)
     .ToList();
    var totalWeight = rounds.Sum(r => r.Config.Weight);
    var roll        = Random.Shared.NextDouble() * totalWeight;
    foreach (var round in rounds) {
      roll -= round.Config.Weight;
      if (roll <= 0) return round;
    }

    throw new InvalidOperationException(
      "Failed to select a special round. This should never happen.");
  }
}