using System.Drawing;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Player;
using TTT.CS2.Extensions;
using TTT.CS2.Hats;
using TTT.CS2.Roles;
using TTT.Game.Events.Player;

namespace TTT.CS2.Listeners;

public class RoleAssignListener(IServiceProvider provider) : IListener {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerConverter<CCSPlayerController> players =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly ITextSpawner? textSpawner =
    provider.GetService<ITextSpawner>();

  public void Dispose() { bus.UnregisterListener(this); }

  [EventHandler(IgnoreCanceled = true)]
  public void OnAssigned(PlayerRoleAssignEvent ev) {
    var player = players.GetPlayer(ev.Player);
    if (player == null || !player.IsValid) return;

    if (player.Team == CsTeam.Spectator) {
      ev.Role = new SpectatorRole(provider);
      return;
    }

    player.SwitchTeam(ev.Role is CS2DetectiveRole ?
      CsTeam.CounterTerrorist :
      CsTeam.Terrorist);

    player.SetClan(ev.Role is CS2DetectiveRole ? ev.Role.Name : "", false);
    var pawn = player.PlayerPawn.Value;
    if (pawn == null || !pawn.IsValid) return;

    var textSettings = new TextSetting {
      msg = ev.Role.Name.First(char.IsAsciiLetter) + "", color = ev.Role.Color
    };
    textSpawner?.CreateTextHat(textSettings, player);
  }
}