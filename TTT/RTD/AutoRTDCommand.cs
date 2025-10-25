using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using JetBrains.Annotations;
using MAULActainShared.plugin.models;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Command;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.CS2.Command;
using TTT.CS2.ThirdParties.eGO;
using TTT.Game.Events.Game;

namespace TTT.RTD;

public class AutoRTDCommand(IServiceProvider provider) : ICommand {
  public string Id => "autortd";
  private ICookie? autoRtdCookie;

  private readonly IPlayerFinder finder =
    provider.GetRequiredService<IPlayerFinder>();

  private readonly ICommandManager commands =
    provider.GetRequiredService<ICommandManager>();

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
  private Dictionary<string, bool> playerStatuses = new();

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
      info.ReplySync("AutoRTD has been disabled.");
    } else {
      await autoRtdCookie.Set(executorId, "1");
      info.ReplySync("AutoRTD has been enabled.");
    }

    playerStatuses[executor.Id] = value != "1";
    return CommandResult.SUCCESS;
  }

  [UsedImplicitly]
  [EventHandler]
  public void OnRoundStart(GameStateUpdateEvent ev) {
    if (ev.NewState != State.WAITING) return;

    Task.Run(async () => {
      foreach (var player in finder.GetOnline()) {
        if (!playerStatuses.TryGetValue(player.Id, out var status)) {
          await fetchCookie(player);
          status = playerStatuses.GetValueOrDefault(player.Id, false);
        }

        if (!status) continue;

        var info = new CS2CommandInfo(provider, player, 0, "css_rtd");
        info.CallingContext = CommandCallingContext.Chat;

        await commands.ProcessCommand(info);
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