using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.Game.Events.Player;

namespace TTT.CS2.GameHandlers;

public class TeamChangeHandler(IServiceProvider provider) : IPluginModule {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

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

    if (int.TryParse(commandInfo.GetArg(1), out var teamIndex))
      requestedTeam = (CsTeam)teamIndex;
    else
      requestedTeam = commandInfo.GetArg(1).ToLower() switch {
        "ct" or "counterterrorist" or "counter"      => CsTeam.CounterTerrorist,
        "t" or "terrorist"                           => CsTeam.Terrorist,
        "s" or "spec" or "spectator" or "spectators" => CsTeam.Spectator,
        _                                            => CsTeam.None
      };

    if (games.ActiveGame is not { State: State.IN_PROGRESS }) {
      if (player != null && player.LifeState != (int)LifeState_t.LIFE_ALIVE)
        Server.NextWorldUpdate(player.Respawn);
      return HookResult.Continue;
    }

    if (requestedTeam is CsTeam.CounterTerrorist or CsTeam.Terrorist)
      if (player != null && player.Team is CsTeam.Spectator or CsTeam.None)
        return HookResult.Continue;

    return HookResult.Handled;
  }

  [UsedImplicitly]
  [GameEventHandler]
  public HookResult OnChangeTeam(EventPlayerTeam ev, GameEventInfo _) {
    if (ev.Userid == null) return HookResult.Continue;
    if (ev.Userid.LifeState == (int)LifeState_t.LIFE_ALIVE)
      return HookResult.Continue;

    var apiPlayer = converter.GetPlayer(ev.Userid);

    var playerDeath = new PlayerDeathEvent(apiPlayer);
    bus.Dispatch(playerDeath);
    return HookResult.Continue;
  }
}