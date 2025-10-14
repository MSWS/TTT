using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Game;
using TTT.API.Role;
using TTT.Game.Listeners;

namespace TTT.CS2.GameHandlers;

public class ChatHandler(IServiceProvider provider) : IPluginModule {
  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  public void Start(BasePlugin? plugin) {
    plugin?.AddCommandListener("say_team", onSay);
  }

  private HookResult onSay(CCSPlayerController? player,
    CommandInfo commandInfo) {
    if (commandInfo.GetCommandString.Contains("test")) return HookResult.Stop;
    return HookResult.Continue;
  }

  public void Dispose() { }
  public void Start() { }
}