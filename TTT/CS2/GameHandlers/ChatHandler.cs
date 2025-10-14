using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Game;
using TTT.API.Messages;
using TTT.API.Player;
using TTT.API.Role;
using TTT.CS2.lang;
using TTT.Game.Listeners;
using TTT.Game.Roles;
using TTT.Locale;

namespace TTT.CS2.GameHandlers;

public class ChatHandler(IServiceProvider provider) : IPluginModule {
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

  public void Start(BasePlugin? plugin) {
    plugin?.AddCommandListener("say_team", onSay);
  }

  private HookResult onSay(CCSPlayerController? player,
    CommandInfo commandInfo) {
    if (player == null
      || converter.GetPlayer(player) is not IOnlinePlayer apiPlayer
      || !roles.GetRoles(apiPlayer).Any(r => r is TraitorRole))
      return HookResult.Continue;

    var teammates = game.ActiveGame?.Players.Where(p
        => roles.GetRoles(p).Any(r => r is TraitorRole))
     .ToList();
    if (teammates == null) return HookResult.Continue;

    var formatted =
      locale[CS2Msgs.TRAITOR_CHAT_FORMAT(apiPlayer, commandInfo.ArgString)];

    foreach (var mate in teammates) messenger.Message(mate, formatted);
    return HookResult.Stop;
  }

  public void Dispose() { }
  public void Start() { }
}