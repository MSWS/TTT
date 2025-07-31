using TTT.API.Events;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.Roles;
using Xunit;

namespace TTT.Test.Game.Roles;

public class RoleAssignerTest(IEventBus bus, IOnlineMessenger messenger,
  IPlayerFinder finder) {
  private readonly RoleAssigner assigner = new(bus, messenger, finder);

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

    var innoRole      = new InnocentRole();
    var traitorRole   = new TraitorRole();
    var detectiveRole = new DetectiveRole();

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
}