using Microsoft.Extensions.DependencyInjection;
using Microsoft.Reactive.Testing;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using Xunit;

namespace TTT.Test.Game.Round;

public class RoundBasedGameTest {
  private readonly IEventBus bus;
  private readonly IPlayerFinder finder;

  private readonly IGame game;

  private readonly IGameManager manager;

  private readonly TestScheduler scheduler;

  public RoundBasedGameTest(IServiceProvider provider) {
    finder    = provider.GetRequiredService<IPlayerFinder>();
    bus       = provider.GetRequiredService<IEventBus>();
    scheduler = provider.GetRequiredService<TestScheduler>();
    manager   = provider.GetRequiredService<IGameManager>();
    game      = manager.CreateGame() ?? throw new InvalidOperationException();
  }

  [Fact]
  public void Start_StartsGame_WithPlayers() {
    finder.AddPlayer(TestPlayer.Random());
    finder.AddPlayer(TestPlayer.Random());

    game.Start(TimeSpan.FromSeconds(5));

    scheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);
    Assert.Equal(State.IN_PROGRESS, game.State);
  }

  [Theory]
  [InlineData(0)]
  [InlineData(1)]
  public void Start_DoesNotStart_WithoutPlayers(int players) {
    for (var i = 0; i < players; i++) finder.AddPlayer(TestPlayer.Random());

    game.Start(TimeSpan.FromSeconds(5));

    scheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

    Assert.Equal(State.WAITING, game.State);
  }

  [Fact]
  public void Start_DoesNotImmediatelyStart_WithoutDelay() {
    finder.AddPlayer(TestPlayer.Random());
    finder.AddPlayer(TestPlayer.Random());

    game.Start(TimeSpan.FromSeconds(5));

    // Advance by less than the delay time
    scheduler.AdvanceBy(TimeSpan.FromSeconds(4).Ticks);

    Assert.Equal(State.COUNTDOWN, game.State);
  }

  [Fact]
  public void StartRound_Stops_IfPlayerLeaves() {
    var player1 = finder.AddPlayer(TestPlayer.Random());

    game.Start(TimeSpan.FromSeconds(5));

    scheduler.AdvanceBy(TimeSpan.FromSeconds(3).Ticks);

    // Simulate player leaving
    finder.RemovePlayer(player1);

    // Advance time to trigger the game logic
    scheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);

    Assert.Equal(State.WAITING, game.State);
  }

  [Fact]
  public void StartRound_StartsImmediately_IfNoCountdown() {
    finder.AddPlayer(TestPlayer.Random());
    finder.AddPlayer(TestPlayer.Random());

    game.Start();

    Assert.Equal(State.IN_PROGRESS, game.State);
  }

  [Fact]
  public void StartRound_NotAssignsRoles_IfNotStarted() {
    finder.AddPlayer(TestPlayer.Random());
    finder.AddPlayer(TestPlayer.Random());

    // Do not start the game
    Assert.Empty(game.Players);

    foreach (var player in game.Players) {
      if (player is not IOnlinePlayer testPlayer)
        throw new InvalidOperationException("Player is not an online player.");

      Assert.Empty(testPlayer.Roles);
    }
  }

  [Fact]
  public void StartRound_AssignsRoles_OnStart() {
    finder.AddPlayer(TestPlayer.Random());
    finder.AddPlayer(TestPlayer.Random());

    game.Start();

    Assert.Equal(2, game.Players.Count);
    foreach (var player in game.Players) {
      if (player is not IOnlinePlayer testPlayer)
        throw new InvalidOperationException("Player is not an online player.");

      Assert.Equal(1, testPlayer.Roles.Count);
    }
  }

  [Fact]
  public void StartRound_AvoidsStarting_IfAlreadyInProgress() {
    finder.AddPlayer(TestPlayer.Random());
    finder.AddPlayer(TestPlayer.Random());

    game.Start(TimeSpan.FromSeconds(5));
    scheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

    var result = game.Start(TimeSpan.FromSeconds(5));

    Assert.Null(result);
  }

  [Fact]
  public void EndGame_ShouldDoNothing_WhenNotInProgress() {
    game.EndGame();

    Assert.Equal(State.WAITING, game.State);
    Assert.Null(game.FinishedAt);
  }
}