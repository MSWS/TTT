using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using Xunit;

namespace TTT.Test.Game.Listeners;

/// <summary>
///   Base GameTest class to reduce boilerplate code in tests.
/// </summary>
/// <param name="provider"></param>
public class GameTest(IServiceProvider provider) {
  protected readonly IEventBus Bus = provider.GetRequiredService<IEventBus>();

  protected readonly IPlayerFinder Finder =
    provider.GetRequiredService<IPlayerFinder>();

  protected readonly IGameManager Games =
    provider.GetRequiredService<IGameManager>();

  protected readonly IServiceProvider Provider = provider;

  protected (IOnlinePlayer alice, IOnlinePlayer bob, IGame game)
    CreateActiveGame(bool start = true) {
    var alice = TestPlayer.Random();
    var bob   = TestPlayer.Random();

    Finder.AddPlayer(alice);
    Finder.AddPlayer(bob);

    var game = Games.CreateGame();
    Assert.NotNull(game);

    if (!start) return (alice, bob, game);

    game.Start();
    return (alice, bob, game);
  }
}