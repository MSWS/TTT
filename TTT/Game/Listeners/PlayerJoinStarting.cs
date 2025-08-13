using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Storage;
using TTT.Game.Events.Player;

namespace TTT.Game.Listeners;

public class PlayerJoinStarting(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly TTTConfig config =
    provider.GetService<IStorage<TTTConfig>>()?.Load().GetAwaiter().GetResult()
    ?? new TTTConfig();

  public override string Name => nameof(PlayerJoinStarting);

  [EventHandler]
  [UsedImplicitly]
  public void OnJoin(PlayerJoinEvent ev) {
    if (Games.IsGameActive()) return;
    var playerCount = Finder.GetOnline().Count;
    if (playerCount < config.RoundCfg.MinimumPlayers) return;

    Messenger.DebugInform(
      $"There are {playerCount} Players online, starting the game...");

    var game = Games.CreateGame();
    game?.Start(config.RoundCfg.CountDownDuration);
  }
}