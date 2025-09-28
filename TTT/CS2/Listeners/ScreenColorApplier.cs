using System.Drawing;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Player;
using TTT.CS2.Extensions;
using TTT.CS2.Roles;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;

namespace TTT.CS2.Listeners;

public class ScreenColorApplier(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  [EventHandler]
  public void OnRoleAssign(PlayerRoleAssignEvent ev) {
    if (ev.Role is SpectatorRole) return;

    var player     = converter.GetPlayer(ev.Player);
    var alphaColor = Color.FromArgb(16, ev.Role.Color);
    if (player != null)
      player.ColorScreen(alphaColor, 5f, 5f,
        flags: PlayerExtensions.FadeFlags.FADE_OUT);

    player?.PrintToCenterHtml("You are a " + ev.Role.Name, 20);
  }
}