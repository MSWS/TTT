using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.Roles;
using Xunit;
using Xunit.Internal;

namespace TTT.Test.Game.Roles;

public class RoleAssignerTest(IServiceProvider provider) {
  private readonly IRoleAssigner assigner =
    provider.GetRequiredService<IRoleAssigner>();

  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  [Fact]
  public void AssignRole_Finishes_WithNoRoles() {
    HashSet<IOnlinePlayer> players = [TestPlayer.Random(), TestPlayer.Random()];

    assigner.AssignRoles(players, [new TestRoles.RoleNever()]);
    Assert.Empty(players.SelectMany(p => p.Roles));
  }

  [Fact]
  public void AssignRole_Finishes_WithNoPlayers() {
    var players = new HashSet<IOnlinePlayer>();

    assigner.AssignRoles(players, []);
    Assert.Empty(players.SelectMany(p => p.Roles));
  }

  [Fact]
  public void AssignRole_AssignsToAllPlayers() {
    HashSet<IOnlinePlayer> players = [TestPlayer.Random(), TestPlayer.Random()];

    assigner.AssignRoles(players, [new TestRoles.RoleGreedy()]);
    Assert.Equal(2, players.SelectMany(p => p.Roles).Count());
    Assert.All(players,
      p => Assert.Equal([new TestRoles.RoleGreedy()], p.Roles));
  }

  // https://www.desmos.com/calculator/d2s9wkztda
  [Theory]
  [InlineData(2, 1, 1, 0)]
  [InlineData(3, 2, 1, 0)]
  [InlineData(4, 3, 1, 0)]
  [InlineData(5, 4, 1, 0)]
  [InlineData(6, 5, 1, 0)]
  [InlineData(7, 5, 2, 0)]
  [InlineData(8, 5, 2, 1)]
  [InlineData(9, 6, 2, 1)]
  [InlineData(10, 7, 2, 1)]
  [InlineData(20, 14, 4, 2)]
  [InlineData(30, 21, 6, 3)]
  [InlineData(32, 21, 7, 4)]
  [InlineData(60, 41, 12, 7)]
  [InlineData(64, 43, 13, 8)]
  public void AssignRole_AssignsBalanced_Roles(int players, int innos,
    int traitors, int detectives) {
    var playerList = new HashSet<IOnlinePlayer>();
    for (var i = 0; i < players; i++) playerList.Add(TestPlayer.Random());

    var innoRole      = new InnocentRole(provider);
    var traitorRole   = new TraitorRole(provider);
    var detectiveRole = new DetectiveRole(provider);

    var roles = new List<IRole>([innoRole, traitorRole, detectiveRole]);
    assigner.AssignRoles(playerList, roles);

    var assignedInnos    = playerList.Count(p => p.Roles.Contains(innoRole));
    var assignedTraitors = playerList.Count(p => p.Roles.Contains(traitorRole));
    var assignedDetectives =
      playerList.Count(p => p.Roles.Contains(detectiveRole));

    Assert.Equal(players, innos + traitors + detectives);
    Assert.Equal(innos, assignedInnos);
    Assert.Equal(traitors, assignedTraitors);
    Assert.Equal(detectives, assignedDetectives);
  }

  [Fact]
  public void AssignRole_DoesNotAssign_IfCanceled() {
    bus.RegisterListener(new RoleAssignCanceler(bus));

    var players = new HashSet<IOnlinePlayer> {
      TestPlayer.Random(), TestPlayer.Random()
    };

    assigner.AssignRoles(players,
      [new TestRoles.RoleA(), new TestRoles.RoleB()]);

    foreach (var player in players)
      Assert.Equal([new TestRoles.RoleB()], player.Roles);
  }

  [Fact]
  public void AssignRole_AssignsRandom_Roles() {
    var firstRoles = new HashSet<IRole>();
    for (var i = 0; i < 100; i++) {
      // Hard-code these to remove all sources of randomness, forcing the
      // assigner to use some source of randomness.
      var firstPlayer = new TestPlayer("first", "first");
      var secondPlayer = new TestPlayer("second", "second");
      var players = new HashSet<IOnlinePlayer> { firstPlayer, secondPlayer };

      assigner.AssignRoles(players,
      [
        new TraitorRole(provider), new InnocentRole(provider),
        new DetectiveRole(provider)
      ]);

      firstRoles.AddRange(firstPlayer.Roles);
      if (firstRoles.Count == 2) return;
    }

    Assert.Fail(
      "First player did not get two different roles after 100 tries.");
  }
}