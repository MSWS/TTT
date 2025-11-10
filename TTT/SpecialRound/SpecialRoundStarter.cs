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

  public List<AbstractSpecialRound> TryStartSpecialRound(
    List<AbstractSpecialRound>? rounds = null) {
    rounds ??= getSpecialRounds();

    Messenger.MessageAll(Locale[RoundMsgs.SPECIAL_ROUND_STARTED(rounds)]);

    foreach (var round in rounds) {
      var roundStart = new SpecialRoundEnableEvent(round);
      Bus.Dispatch(roundStart);
      Messenger.MessageAll(Locale[round.Description]);
      round.ApplyRoundEffects();
    }

    tracker.ActiveRounds.AddRange(rounds);
    tracker.RoundsSinceLastSpecial = 0;
    return rounds;
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

    var specialRound = getSpecialRounds();

    TryStartSpecialRound(specialRound);
  }

  private List<AbstractSpecialRound> getSpecialRounds() {
    var selectedRounds = new List<AbstractSpecialRound>();

    do {
      var round = pickWeightedRound(selectedRounds);
      if (round == null) break;
      selectedRounds.Add(round);
    } while (config.MultiRoundChance > Random.Shared.NextSingle());

    return selectedRounds;
  }

  private AbstractSpecialRound? pickWeightedRound(
    List<AbstractSpecialRound> exclude) {
    var rounds = Provider.GetServices<ITerrorModule>()
     .OfType<AbstractSpecialRound>()
     .Where(r => r.Config.Weight > 0 && !exclude.Contains(r))
     .Where(r => !exclude.Any(er => er.ConflictsWith(r) || r.ConflictsWith(er)))
     .ToList();
    if (rounds.Count == 0) return null;
    var totalWeight = rounds.Sum(r => r.Config.Weight);
    var roll        = Random.Shared.NextDouble() * totalWeight;
    foreach (var round in rounds) {
      roll -= round.Config.Weight;
      if (roll <= 0) return round;
    }

    return null;
  }
}