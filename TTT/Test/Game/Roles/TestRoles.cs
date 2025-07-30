using System.Drawing;
using TTT.API;
using TTT.API.Player;
using TTT.API.Role;

namespace TTT.Test.Game.Roles;

public class TestRoles {
  public class RoleNever : IRole {
    public string Id { get; } = "test.role.never";
    public string Name { get; } = "Never Assigned";
    public Color Color { get; } = Color.Red;

    public IOnlinePlayer? FindPlayerToAssign(ISet<IOnlinePlayer> players) {
      return null;
    }
  }

  public class RoleGreedy : IRole {
    public string Id { get; } = "test.role.always";
    public string Name { get; } = "Always Assigned";
    public Color Color { get; } = Color.Green;

    public IOnlinePlayer? FindPlayerToAssign(ISet<IOnlinePlayer> players) {
      return players.FirstOrDefault(p => p.Roles.All(r => r.Id != Id));
    }
  }

  public class RoleA : IRole {
    public string Id { get; } = "test.role.a";
    public string Name { get; } = "Role A";
    public Color Color { get; } = Color.Blue;

    public IOnlinePlayer? FindPlayerToAssign(ISet<IOnlinePlayer> players) {
      return players.FirstOrDefault(p => p.Roles.All(r => r.Id != Id));
    }
  }

  public class RoleB : IRole {
    public string Id { get; } = "test.role.b";
    public string Name { get; } = "Role B";
    public Color Color { get; } = Color.Yellow;

    public IOnlinePlayer? FindPlayerToAssign(ISet<IOnlinePlayer> players) {
      return players.FirstOrDefault(p => p.Roles.All(r => r.Id != Id));
    }
  }
}