using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using MAULActainShared.plugin;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.API.Role;
using TTT.CS2.lang;
using TTT.CS2.ThirdParties.eGO;
using TTT.Game.Roles;
using TTT.Locale;

namespace TTT.CS2.GameHandlers;

public class TraitorChatHandler(IServiceProvider provider) : IPluginModule {
  private readonly IGameManager game =
    provider.GetRequiredService<IGameManager>();

  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IMessenger messenger =
    provider.GetRequiredService<IMessenger>();

  private readonly IMsgLocalizer locale =
    provider.GetRequiredService<IMsgLocalizer>();

  private IActain? maulService = null;

  public void Start(BasePlugin? plugin) {
    try {
      maulService ??= EgoApi.MAUL.Get();
      if (maulService != null) {
        maulService.getChatShareService().OnChatShare += OnOnChatShare;
        return;
      }

      plugin?.AddCommandListener("say_team", onSay);
    } catch (KeyNotFoundException) {
      plugin?.AddCommandListener("say_team", onSay);
    }
  }

  private void OnOnChatShare(CCSPlayerController? player, CommandInfo info,
    ref bool canceled) {
    if (!info.GetArg(0).Equals("say_team", StringComparison.OrdinalIgnoreCase))
      return;
    var result                                 = onSay(player, info);
    if (result == HookResult.Handled) canceled = true;
  }

  private HookResult onSay(CCSPlayerController? player,
    CommandInfo commandInfo) {
    if (player == null
      || game.ActiveGame is not { State: State.IN_PROGRESS or State.FINISHED }
      || converter.GetPlayer(player) is not IOnlinePlayer apiPlayer
      || !roles.GetRoles(apiPlayer).Any(r => r is TraitorRole))
      return HookResult.Continue;

    var teammates = game.ActiveGame?.Players.Where(p
        => roles.GetRoles(p).Any(r => r is TraitorRole))
     .ToList();
    if (teammates == null) return HookResult.Continue;

    var msg = commandInfo.ArgString;
    if (msg.StartsWith('"') && msg.EndsWith('"') && msg.Length >= 2)
      msg = msg[1..^1];
    var formatted = locale[CS2Msgs.TRAITOR_CHAT_FORMAT(apiPlayer, msg)];

    foreach (var mate in teammates) messenger.Message(mate, formatted);
    return HookResult.Handled;
  }

  public void Dispose() {
    if (maulService != null)
      maulService.getChatShareService().OnChatShare -= OnOnChatShare;
  }

  public void Start() { }
}