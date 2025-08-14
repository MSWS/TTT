using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Role;
using TTT.API.Storage;
using TTT.CS2.Extensions;
using TTT.CS2.Utils;
using TTT.Game;
using TTT.Game.Events.Game;
using TTT.Game.Roles;

namespace TTT.CS2.Listeners;

public class RoundTimerListener(IServiceProvider provider) : IListener {
  private readonly IEventBus bus = provider.GetRequiredService<IEventBus>();

  private readonly TTTConfig config = provider
   .GetRequiredService<IStorage<TTTConfig>>()
   .Load()
   .GetAwaiter()
   .GetResult() ?? new TTTConfig();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  private readonly IRoleAssigner roles =
    provider.GetRequiredService<IRoleAssigner>();

  public void Dispose() { bus.UnregisterListener(this); }

  [UsedImplicitly]
  [EventHandler(IgnoreCanceled = true)]
  public void OnRoundStart(GameStateUpdateEvent ev) {
    if (ev.NewState == State.COUNTDOWN) {
      Server.NextWorldUpdate(() => {
        RoundUtil.SetTimeRemaining((int)config.RoundCfg.CountDownDuration
         .TotalSeconds);
        Server.ExecuteCommand("mp_ignore_round_win_conditions 1");
        foreach (var player in Utilities.GetPlayers()
         .Where(p => p.LifeState != (int)LifeState_t.LIFE_ALIVE)) {
          player.PawnIsAlive = true;
          player.Respawn();
          Utilities.SetStateChanged(player, "CCSPlayerController",
            "m_bPawnIsAlive");
        }

        foreach (var player in Utilities.GetPlayers())
          player.SetColor(Color.FromArgb(254, 255, 255, 255));
      });

      return;
    }

    if (ev.NewState != State.IN_PROGRESS) return;
    Server.NextWorldUpdate(() => {
      RoundUtil.SetTimeRemaining((int)config.RoundCfg
       .RoundDuration(ev.Game.Players.Count)
       .TotalSeconds);
      Server.ExecuteCommand("mp_ignore_round_win_conditions 0");
    });
  }

  [UsedImplicitly]
  [EventHandler(IgnoreCanceled = true)]
  public void OnRoundEnd(GameStateUpdateEvent ev) {
    if (ev.NewState != State.FINISHED) return;
    if (RoundUtil.GetTimeRemaining() <= 1) return;

    foreach (var player in ev.Game.Players) {
      var csPlayer = converter.GetPlayer(player);
      if (csPlayer == null || !csPlayer.IsValid) continue;
      var role = roles.GetRoles(player).FirstOrDefault();
      if (role == null) continue;
      csPlayer.SetClan(role.Name, false);
    }

    foreach (var inno in ev.Game.GetAlive(typeof(InnocentRole))) {
      var player = converter.GetPlayer(inno);
      player?.SwitchTeam(CsTeam.CounterTerrorist);
    }

    new EventNextlevelChanged(true).FireEvent(false);

    var endReason =
      ev.Game.WinningRole != null && ev.Game.WinningRole.GetType()
       .IsAssignableTo(typeof(TraitorRole)) ?
        RoundEndReason.TerroristsWin :
        RoundEndReason.CTsWin;

    RoundUtil.EndRound(endReason);
  }
}