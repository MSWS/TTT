using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
using TTT.API.Player;
using TTT.Game.Listeners;
using TTT.Karma;
using TTT.Karma.Events;

namespace TTT.CS2.Listeners;

public class KarmaSyncer(IServiceProvider provider)
  : BaseListener(provider), IPluginModule {
  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IKarmaService? karma = provider.GetService<IKarmaService>();

  [UsedImplicitly]
  [EventHandler]
  public void OnKarmaUpdate(KarmaUpdateEvent ev) {
    if (karma == null) return;

    Server.NextWorldUpdate(() => {
      var player = converter.GetPlayer(ev.Player);
      if (player == null) return;

      player.Score = ev.Karma;
      Utilities.SetStateChanged(player, "CCSPlayerController", "m_iScore");
    });
  }

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnJoin(EventPlayerConnectFull ev, GameEventInfo _) {
    if (ev.Userid == null || karma == null) return HookResult.Continue;
    var player = converter.GetPlayer(ev.Userid);
    var user   = ev.Userid;

    Task.Run(async () => {
      var karmaValue = await karma.Load(player);
      await Server.NextWorldUpdateAsync(() => {
        if (!user.IsValid) return;
        user.Score = karmaValue;
        Utilities.SetStateChanged(user, "CCSPlayerController", "m_iScore");
      });
    });

    return HookResult.Continue;
  }
}