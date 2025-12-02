using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Commands;
using JetBrains.Annotations;
using MAULActainShared.plugin.models;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Command;
using TTT.API.Events;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.CS2.Command;
using TTT.CS2.ThirdParties.eGO;
using TTT.Game.Events.Game;
using TTT.Locale;
using TTT.RTD.lang;

namespace TTT.RTD;

public class AutoRTDCommand(IServiceProvider provider) : ICommand, IListener {
  private readonly ICommandManager commands =
    provider.GetRequiredService<ICommandManager>();

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly IMsgLocalizer localizer =
    provider.GetRequiredService<IMsgLocalizer>();

  private readonly IPermissionManager perms =
    provider.GetRequiredService<IPermissionManager>();

  private readonly Dictionary<string, bool> playerStatuses = new();
  private ICookie? autoRtdCookie;
  public string Id => "autortd";

  public bool MustBeOnMainThread => true;

  public void Dispose() { }

  public void Start() {
    Task.Run(async () => {
      var api = EgoApi.MAUL.Get();
      if (api != null) {
        await api.getCookieService().RegClientCookie("ttt_autortd");
        autoRtdCookie =
          await api.getCookieService().FindClientCookie("ttt_autortd");
      }
    });
  }

  public string[] RequiredFlags => ["@ttt/autortd"];

  public async Task<CommandResult> Execute(IOnlinePlayer? executor,
    ICommandInfo info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;
    if (autoRtdCookie == null) {
      info.ReplySync("AutoRTD system is not available.");
      return CommandResult.SUCCESS;
    }

    if (!ulong.TryParse(executor.Id, out var executorId)) {
      info.ReplySync("Your player ID is invalid for AutoRTD.");
      return CommandResult.SUCCESS;
    }

    var value = await autoRtdCookie.Get(executorId);
    if (value == "1") {
      await autoRtdCookie.Set(executorId, "0");
      info.ReplySync(localizer[RtdMsgs.COMMAND_AUTORTD_DISABLED]);
    } else {
      await autoRtdCookie.Set(executorId, "1");
      info.ReplySync(localizer[RtdMsgs.COMMAND_AUTORTD_ENABLED]);
    }

    playerStatuses[executor.Id] = value != "1";
    return CommandResult.SUCCESS;
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnRoundStart(GameInitEvent ev) {
    Task.Run(async () => {
      foreach (var player in finder.GetOnline()) {
        if (!perms.HasFlags(player, RequiredFlags)) continue;

        if (!playerStatuses.TryGetValue(player.Id, out var status)) {
          await fetchCookie(player);
          status = playerStatuses.GetValueOrDefault(player.Id, false);
        }

        if (!status) continue;

        var info = new CS2CommandInfo(provider, player, 0, "css_rtd") {
          CallingContext = CommandCallingContext.Chat
        };

        await Server.NextWorldUpdateAsync(() => commands.ProcessCommand(info));
      }
    });
  }

  private async Task fetchCookie(IPlayer player) {
    if (autoRtdCookie == null) return;
    if (!ulong.TryParse(player.Id, out var playerId)) return;

    var value = await autoRtdCookie.Get(playerId);
    playerStatuses[player.Id] = value == "1";
  }
}