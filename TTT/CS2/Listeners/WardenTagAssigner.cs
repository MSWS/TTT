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
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly Dictionary<string, (string, char)> oldTags = new();

  [UsedImplicitly]
  [EventHandler]
  public void OnRoleAssign(PlayerRoleAssignEvent ev) {
    var maul = EgoApi.MAUL.Get();
    if (maul == null) return;
    Server.NextWorldUpdate(() => {
      var gamePlayer = converter.GetPlayer(ev.Player);
      if (gamePlayer == null) return;

      Task.Run(async () => {
        if (ev.Role is DetectiveRole) {
          var oldTag = await maul.getTagService().GetTag(gamePlayer.SteamID);
          var oldTagColor =
            await maul.getTagService().GetTagColor(gamePlayer.SteamID);
          oldTags[ev.Player.Id] = (oldTag, oldTagColor);
        }

        await Server.NextWorldUpdateAsync(() => {
          if (ev.Role is DetectiveRole) {
            maul.getTagService().SetTag(gamePlayer, "[DETECTIVE]", false);
            maul.getTagService()
             .SetTagColor(gamePlayer, ChatColors.DarkBlue, false);
          } else if (oldTags.TryGetValue(ev.Player.Id, out var oldTag)) {
            maul.getTagService().SetTag(gamePlayer, oldTag.Item1, false);
            maul.getTagService().SetTagColor(gamePlayer, oldTag.Item2, false);

            oldTags.Remove(ev.Player.Id);
          }
        });
      });
    });
  }
}