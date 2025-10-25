using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
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

  private SpecialRoundsConfig config
    => Provider.GetService<IStorage<SpecialRoundsConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new SpecialRoundsConfig();

  private int roundsSinceMapChange = 0;

  public void Start(BasePlugin? plugin) {
    plugin?.RegisterListener<Listeners.OnMapStart>(onMapChange);
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
    return Provider.GetServices<ITerrorModule>()
     .OfType<AbstractSpecialRound>()
     .OrderBy(_ => Random.Shared.Next())
     .First();
  }

  public AbstractSpecialRound?
    TryStartSpecialRound(AbstractSpecialRound? round) {
    round ??= getSpecialRound();
    Messenger.MessageAll(Locale[RoundMsgs.SPECIAL_ROUND_STARTED(round)]);
    Messenger.MessageAll(Locale[round.Description]);

    round?.ApplyRoundEffects();
    tracker.CurrentRound           = round;
    tracker.RoundsSinceLastSpecial = 0;
    return round;
  }
}