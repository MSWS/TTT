using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Player;
using TTT.Game.Roles;

namespace TTT.CS2.Roles;

public class CS2TraitorRole(IServiceProvider provider) : TraitorRole(provider) {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  public override void OnAssign(IOnlinePlayer player) {
    base.OnAssign(player);

    var gamePlayer = converter.GetPlayer(player);
    if (gamePlayer == null) return;

    gamePlayer.AcceptInput("SetTargetName", null, null, "traitor");
    if (gamePlayer.Pawn.Value != null) gamePlayer.Pawn.Value.Target = "traitor";
  }
}