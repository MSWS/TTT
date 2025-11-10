using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI;
using ShopAPI.Events;
using SpecialRound.lang;
using SpecialRoundAPI;
using SpecialRoundAPI.Configs;
using TTT.API.Events;
using TTT.API.Storage;
using TTT.Game.Events.Game;
using TTT.Locale;

namespace SpecialRound.Rounds;

public class RichRound(IServiceProvider provider)
  : AbstractSpecialRound(provider) {
  public override string Name => "Rich";
  public override IMsg Description => RoundMsgs.SPECIAL_ROUND_RICH;
  public override SpecialRoundConfig Config => config;

  private RichRoundConfig config
    => Provider.GetService<IStorage<RichRoundConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new RichRoundConfig();

  public override void ApplyRoundEffects() { }

  [UsedImplicitly]
  [EventHandler]
  public void OnBalanceChange(PlayerBalanceEvent ev) {
    if (!Tracker.ActiveRounds.Contains(this)) return;
    if (ev.Reason == "Round Start") {
      var newBal = (int)(ev.NewBalance * config.BonusCreditsMultiplier);
      ev.NewBalance = newBal;
      return;
    }

    if (ev.NewBalance <= ev.OldBalance) return;

    var gain = ev.NewBalance - ev.OldBalance;
    gain          = (int)(gain * config.AdditiveCreditsMultiplier);
    ev.NewBalance = ev.OldBalance + gain;
  }

  public override bool ConflictsWith(AbstractSpecialRound other) {
    return other is VanillaRound;
  }

  public override void OnGameState(GameStateUpdateEvent ev) { }
}