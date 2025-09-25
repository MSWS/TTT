using System.Drawing;
using System.Reactive.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using TTT.API.Events;
using TTT.API.Game;
using TTT.API.Player;
using TTT.API.Storage;
using TTT.CS2.Extensions;
using TTT.CS2.Utils;
using TTT.Game;
using TTT.Game.Events.Game;
using TTT.Game.Listeners;
using TTT.Game.Roles;

namespace TTT.CS2.Listeners;

public class RoundTimerListener(IServiceProvider provider)
  : BaseListener(provider) {
  private readonly TTTConfig config = provider
   .GetRequiredService<IStorage<TTTConfig>>()
   .Load()
   .GetAwaiter()
   .GetResult() ?? new TTTConfig();

  private readonly IPlayerConverter<CCSPlayerController> converter =
    provider.GetRequiredService<IPlayerConverter<CCSPlayerController>>();

  [UsedImplicitly]
  [EventHandler(IgnoreCanceled = true)]
  public void OnRoundStart(GameStateUpdateEvent ev) {
    if (ev.NewState == State.COUNTDOWN) {
      Server.NextWorldUpdate(() => {
        RoundUtil.SetTimeRemaining((int)config.RoundCfg.CountDownDuration
         .TotalSeconds);
        Server.ExecuteCommand("mp_ignore_round_win_conditions 1");
        foreach (var player in Utilities.GetPlayers()
         .Where(p => p.LifeState != (int)LifeState_t.LIFE_ALIVE))
          player.Respawn();

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

    revealRoles(ev.Game);

    if (RoundUtil.GetTimeRemaining() <= 1) return;

    Server.NextWorldUpdate(() => {
      var endReason = endRound(ev);

      var timer = Observable.Timer(
        config.RoundCfg.TimeBetweenRounds, Scheduler);
      timer.Subscribe(_
        => Server.NextWorldUpdate(() => RoundUtil.EndRound(endReason)));
    });
  }

  private RoundEndReason endRound(GameStateUpdateEvent ev) {
    var endReason =
      ev.Game.WinningRole != null && ev.Game.WinningRole.GetType()
       .IsAssignableTo(typeof(TraitorRole)) ?
        RoundEndReason.TerroristsWin :
        RoundEndReason.CTsWin;

    var panelWinEvent = new EventCsWinPanelRound(true);
    var winningTeam = endReason == RoundEndReason.TerroristsWin ?
      CsTeam.Terrorist :
      CsTeam.CounterTerrorist;
    panelWinEvent.Set("final_event",
      winningTeam == CsTeam.CounterTerrorist ? 2 : 3);
    panelWinEvent.FireEvent(false);
    return endReason;
  }

  private void revealRoles(IGame game) {
    foreach (var player in game.Players) {
      var csPlayer = converter.GetPlayer(player);
      if (csPlayer == null || !csPlayer.IsValid) continue;
      var role = Roles.GetRoles(player).FirstOrDefault();
      if (role == null) continue;
      csPlayer.SetClan(role.Name, false);
      if (role is InnocentRole) csPlayer.SwitchTeam(CsTeam.CounterTerrorist);
    }

    new EventNextlevelChanged(true).FireEvent(false);
  }
}