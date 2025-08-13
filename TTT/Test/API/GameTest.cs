using Microsoft.Extensions.DependencyInjection;
using TTT.API.Game;
using TTT.API.Role;
using TTT.Test.Game.Roles;
using Xunit;

namespace TTT.Test.API;

public class GameTest(IServiceProvider provider) {
  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  [Fact]
  public void GetAlive_OnlyReturnsAlivePlayers() {
    // Arrange
    var game    = provider.GetRequiredService<IGameManager>().CreateGame()!;
    var player1 = new TestPlayer("player1", "Player 1") { IsAlive = true };
    var player2 = new TestPlayer("player2", "Player 2") { IsAlive = false };
    game.Players.Add(player1);
    game.Players.Add(player2);

    // Act
    var alivePlayers = game.GetAlive();

    // Assert
    Assert.Single(alivePlayers);
    Assert.Contains(player1, alivePlayers);
  }

  [Fact]
  public void GetAlive_WithRoleInstance_ReturnsCorrectPlayers() {
    // Arrange
    var game    = provider.GetRequiredService<IGameManager>().CreateGame()!;
    var player1 = new TestPlayer("player1", "Player 1") { IsAlive = true };
    var player2 = new TestPlayer("player2", "Player 2") { IsAlive = true };

    var roleA = new TestRoles.RoleA(roles);
    roles.Write(player1, [roleA]);
    roles.Write(player2, [new TestRoles.RoleB(roles)]);
    game.Players.Add(player1);
    game.Players.Add(player2);

    // Act
    var aliveWithRoleA = game.GetAlive(roleA);

    // Assert
    Assert.Single(aliveWithRoleA);
    Assert.Contains(player1, aliveWithRoleA);
  }

  [Fact]
  public void GetAlive_WithRoleType_ReturnsCorrectPlayers() {
    // Arrange
    var game    = provider.GetRequiredService<IGameManager>().CreateGame()!;
    var player1 = new TestPlayer("player1", "Player 1") { IsAlive = true };
    var player2 = new TestPlayer("player2", "Player 2") { IsAlive = true };
    game.Players.Add(player1);
    game.Players.Add(player2);
    roles.Write(player1, [new TestRoles.RoleA(roles)]);
    roles.Write(player2, [new TestRoles.RoleB(roles)]);

    // Act
    var aliveWithRoleA = game.GetAlive(typeof(TestRoles.RoleA));

    // Assert
    Assert.Single(aliveWithRoleA);
    Assert.Contains(player1, aliveWithRoleA);
  }
}