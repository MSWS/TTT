using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Game;

namespace TTT.CS2.GameHandlers;

public class TeamChangeHandler(IServiceProvider provider) : IPluginModule {
  private readonly IGameManager games =
    provider.GetRequiredService<IGameManager>();

  public void Dispose() { }
  public void Start() { }

  public void Start(BasePlugin? plugin) {
    plugin?.AddCommandListener("jointeam", onJoinTeam);
  }

  private HookResult onJoinTeam(CCSPlayerController? player,
    CommandInfo commandInfo) {
    CsTeam requestedTeam;

    if (int.TryParse(commandInfo.GetArg(1), out var teamIndex)) {
      requestedTeam = (CsTeam)teamIndex;
    } else {
      requestedTeam = commandInfo.GetArg(1).ToLower() switch {
        "ct" or "counterterrorist" or "counter"      => CsTeam.CounterTerrorist,
        "t" or "terrorist"                           => CsTeam.Terrorist,
        "s" or "spec" or "spectator" or "spectators" => CsTeam.Spectator,
        _                                            => CsTeam.None
      };
    }

    if (requestedTeam is CsTeam.CounterTerrorist or CsTeam.Terrorist) {
      if (player != null && player.Team is CsTeam.Spectator or CsTeam.None) {
        Server.NextWorldUpdate(player.Respawn);
        return HookResult.Continue;
      }

      return HookResult.Handled;
    }

    if (games.ActiveGame is not { State: State.IN_PROGRESS })
      return HookResult.Continue;

    return HookResult.Handled;
  }
}