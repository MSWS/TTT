using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using ShopAPI.Events;
using SpecialRound.lang;
using SpecialRoundAPI;
using TTT.API.Events;
using TTT.API.Messages;
using TTT.API.Storage;
using TTT.Game.Events.Game;
using TTT.Locale;

namespace SpecialRound.Rounds;

public class VanillaRound(IServiceProvider provider)
  : AbstractSpecialRound(provider) {
  public override string Name => "Vanilla";
  public override IMsg Description => RoundMsgs.SPECIAL_ROUND_VANILLA;

  private readonly IMessenger messenger =
    provider.GetRequiredService<IMessenger>();

  private readonly IMsgLocalizer locale =
    provider.GetRequiredService<IMsgLocalizer>();

  private VanillaRoundConfig config
    => Provider.GetService<IStorage<VanillaRoundConfig>>()
    ?.Load()
     .GetAwaiter()
     .GetResult() ?? new VanillaRoundConfig();

  public override SpecialRoundConfig Config => config;

  public override void ApplyRoundEffects() { }

  public override void OnGameState(GameStateUpdateEvent ev) { }

  [UsedImplicitly]
  [EventHandler(Priority = Priority.HIGH)]
  public void OnPurchase(PlayerPurchaseItemEvent ev) {
    if (Tracker.CurrentRound != this) return;
    ev.IsCanceled = true;

    messenger.Message(ev.Player, locale[RoundMsgs.VANILLA_ROUND_REMINDER]);
  }
}