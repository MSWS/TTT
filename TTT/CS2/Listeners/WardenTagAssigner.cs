using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Player;
using TTT.CS2.ThirdParties.eGO;
using TTT.Game.Events.Player;
using TTT.Game.Listeners;
using TTT.Game.Roles;

namespace TTT.CS2.Listeners;

public class WardenTagAssigner(IServiceProvider provider)
  : BaseListener(provider) {
  private Dictionary<string, string> oldTags = new();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  [UsedImplicitly]
  [EventHandler]
  public void OnRoleAssign(PlayerRoleAssignEvent ev) {
    var maul = EgoApi.MAUL.Get();
    if (maul == null) return;
    Server.NextWorldUpdate(() => {
      var gamePlayer = converter.GetPlayer(ev.Player);
      if (gamePlayer == null) return;

      Task.Run(async () => {
        if (ev.Role is DetectiveRole)
          oldTags[ev.Player.Id] =
            await maul.getTagService().GetTag(gamePlayer.SteamID);

        await Server.NextWorldUpdateAsync(() => {
          if (ev.Role is DetectiveRole) {
            maul.getTagService().SetTag(gamePlayer, "[DETECTIVE]", false);
            maul.getTagService()
             .SetTagColor(gamePlayer, ChatColors.LightBlue, false);
          } else if (oldTags.TryGetValue(ev.Player.Id, out var oldTag)) {
            maul.getTagService().SetTag(gamePlayer, oldTag, false);
            maul.getTagService()
             .SetTagColor(gamePlayer, ChatColors.White, false);

            oldTags.Remove(ev.Player.Id);
          }
        });
      });
    });
  }
}