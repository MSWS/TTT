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
    Assert.Empty(players.SelectMany(p => assigner.GetRoles(p)));
  }

  [Fact]
  public void AssignRole_Finishes_WithNoPlayers() {
    var players = new HashSet<IOnlinePlayer>();

    assigner.AssignRoles(players, []);
    Assert.Empty(players.SelectMany(p => assigner.GetRoles(p)));
  }

  [Fact]
  public void AssignRole_AssignsToAllPlayers() {
    HashSet<IOnlinePlayer> players = [TestPlayer.Random(), TestPlayer.Random()];

    assigner.AssignRoles(players, [new TestRoles.RoleGreedy(assigner)]);
    Assert.Equal(2, players.SelectMany(p => assigner.GetRoles(p)).Count());
    Assert.All(players,
      p => Assert.Equal([new TestRoles.RoleGreedy(assigner)],
        assigner.GetRoles(p)));
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
  [InlineData(30, 22, 6, 2)]
  [InlineData(32, 22, 7, 3)]
  [InlineData(60, 43, 12, 5)]
  [InlineData(64, 45, 13, 6)]
  public void AssignRole_AssignsBalanced_Roles(int players, int innos,
    int traitors, int detectives) {
    var playerList = new HashSet<IOnlinePlayer>();
    for (var i = 0; i < players; i++) playerList.Add(TestPlayer.Random());

    var innoRole      = new InnocentRole(provider);
    var traitorRole   = new TraitorRole(provider);
    var detectiveRole = new DetectiveRole(provider);

    var roles = new List<IRole>([innoRole, traitorRole, detectiveRole]);
    assigner.AssignRoles(playerList, roles);

    var assignedInnos =
      playerList.Count(p => assigner.GetRoles(p).Contains(innoRole));
    var assignedTraitors =
      playerList.Count(p => assigner.GetRoles(p).Contains(traitorRole));
    var assignedDetectives =
      playerList.Count(p => assigner.GetRoles(p).Contains(detectiveRole));

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
      [new TestRoles.RoleA(assigner), new TestRoles.RoleB(assigner)]);

    foreach (var player in players)
      Assert.Equal([new TestRoles.RoleB(assigner)], assigner.GetRoles(player));
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

      firstRoles.AddRange(assigner.GetRoles(firstPlayer));
      if (firstRoles.Count == 2) return;
    }

    Assert.Fail(
      "First player did not get two different roles after 100 tries.");
  }
}