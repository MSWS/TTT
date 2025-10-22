using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Role;
using TTT.Game.Events.Player;

namespace Stats;

public class KillListener(IServiceProvider provider) : IListener {
  private readonly HttpClient client =
    provider.GetRequiredService<HttpClient>();

  private readonly IGameManager game =
    provider.GetRequiredService<IGameManager>();

  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  public void Dispose() { }

  [UsedImplicitly]
  [EventHandler]
  public void OnPlayerKill(PlayerDeathEvent ev) {
    if (game.ActiveGame is not { State: State.IN_PROGRESS }) return;
    if (ev.Killer == null) return;

    var killerRole = roles.GetRoles(ev.Killer).FirstOrDefault();
    if (killerRole == null) return;
    var victimRole = roles.GetRoles(ev.Victim).FirstOrDefault();
    if (victimRole == null) return;

    var data = new {
      killer_steam_id = ev.Killer.Id,
      victim_steam_id = ev.Victim.Id,
      killer_role     = StatsApi.ApiNameForRole(killerRole),
      victim_role     = StatsApi.ApiNameForRole(victimRole),
      weapon          = ev.Weapon
    };

    var payload = new StringContent(
      System.Text.Json.JsonSerializer.Serialize(data),
      System.Text.Encoding.UTF8, "application/json");

    Task.Run(async () => { await client.PostAsync("kill", payload); });
  }
}