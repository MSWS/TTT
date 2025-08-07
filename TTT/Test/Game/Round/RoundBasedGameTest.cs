using Microsoft.Extensions.DependencyInjection;
using Microsoft.Reactive.Testing;
using TTT.API.Game;
using TTT.API.Player;
using Xunit;

namespace TTT.Test.Game.Round;

public class RoundBasedGameTest {
  private readonly IPlayerFinder finder;

  private readonly IGame game;

  private readonly TestScheduler scheduler;

  public RoundBasedGameTest(IServiceProvider provider) {
    finder    = provider.GetRequiredService<IPlayerFinder>();
    scheduler = provider.GetRequiredService<TestScheduler>();
    var manager = provider.GetRequiredService<IGameManager>();
    game = manager.CreateGame() ?? throw new InvalidOperationException();
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

  [Fact]
  public void StartRound_PrintsRoles_WithIs() {
    var player1 = finder.AddPlayer(TestPlayer.Random()) as TestPlayer;
    finder.AddPlayer(TestPlayer.Random());

    Assert.NotNull(player1);
    game.Start();

    Assert.Contains(player1.Messages,
      m => stripChatColors(m).Contains("1 Traitor"));
    Assert.Contains(player1.Messages, m => m.Contains(" is "));
  }

  [Fact]
  public void StartRound_PrintsRoles_WithAre() {
    var player1 = finder.AddPlayer(TestPlayer.Random()) as TestPlayer;
    for (var i = 0; i < 6; i++) finder.AddPlayer(TestPlayer.Random());

    Assert.NotNull(player1);
    game.Start();

    Assert.Contains(player1.Messages,
      m => stripChatColors(m).Contains("2 Traitors"));
    Assert.Contains(player1.Messages, m => m.Contains(" are "));
  }

  private static string stripChatColors(string s) {
    return s.Replace("\x01", "")
     .Replace("\x02", "")
     .Replace("\x03", "")
     .Replace("\x04", "")
     .Replace("\x05", "")
     .Replace("\x06", "")
     .Replace("\x07", "")
     .Replace("\x08", "")
     .Replace("\x09", "")
     .Replace("\x0A", "")
     .Replace("\x0B", "")
     .Replace("\x0C", "")
     .Replace("\x0D", "")
     .Replace("\x0E", "")
     .Replace("\x0F", "")
     .Replace("\x10", "");
  }
}