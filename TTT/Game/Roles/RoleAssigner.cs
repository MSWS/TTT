using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.API.Role;
using TTT.Game.Events.Player;

namespace TTT.Game.Roles;

public class RoleAssigner(IServiceProvider provider) : IRoleAssigner {
  private static readonly Random rng = new();

  private readonly IDictionary<string, ICollection<IRole>> assignedRoles =
    new Dictionary<string, ICollection<IRole>>();

  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IMessenger? onlineMessenger =
    provider.GetService<IMessenger>();

  public void AssignRoles(ISet<IOnlinePlayer> players, IList<IRole> roles) {
    assignedRoles.Clear();
    var  shuffled = players.OrderBy(_ => rng.NextDouble()).ToHashSet();
    bool roleAssigned;
    do { roleAssigned = tryAssignRole(shuffled, roles); } while (roleAssigned);
  }

  public Task<ICollection<IRole>?> Load(IPlayer key) {
    assignedRoles.TryGetValue(key.Id, out var roles);
    return Task.FromResult(roles);
  }

  public Task Write(IPlayer key, ICollection<IRole> newData) {
    assignedRoles[key.Id] = newData;
    return Task.CompletedTask;
  }

  private bool tryAssignRole(HashSet<IOnlinePlayer> players,
    IList<IRole> roles) {
    foreach (var role in roles) {
      var player = role.FindPlayerToAssign(players);
      if (player is null) continue;

      var ev = new PlayerRoleAssignEvent(player, role);
      bus.Dispatch(ev);

      if (ev.IsCanceled) continue;

      if (!assignedRoles.ContainsKey(player.Id))
        assignedRoles[player.Id] = new List<IRole>();
      assignedRoles[player.Id].Add(ev.Role);
      ev.Role.OnAssign(player);

      onlineMessenger?.Debug(
        $"{player.Name} was assigned the role of {role.Name}.");
      return true;
    }

    return false;
  }
}